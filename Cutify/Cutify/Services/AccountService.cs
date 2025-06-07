using AutoMapper;
using Cutify.Data;
using Cutify.Helper;
using Cutify.Models;
using Cutify.Services.Interface;
using Cutify.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;

        public AccountService(AppDbContext context, IMapper mapper, IEmailService emailService, IFileService fileService, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _fileService = fileService;
            _env = env;
        }

        public async Task<(bool Succeeded, string ErrorMessage)> LoginAsync(LoginVM loginVM, HttpContext httpContext)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginVM.Email );

            if (user == null || !PasswordHash.VerifyHashedPassword(user.Password, loginVM.Password))
            {
                return (false, "Invalid login attempt.");
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("UserId", user.Id.ToString())
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            return (true, null);
        }

        public async Task<(bool Succeeded, string ErrorMessage)> RegisterAsync(RegisterVM registerVM, HttpContext httpContext)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerVM.Email);
            if (existingUser != null)
            {
                return (false, "Bu email artıq istifadə olunur.");
            }

            string fileName = Guid.NewGuid() + Path.GetExtension(registerVM.Image.FileName);
            string path = Path.Combine(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await registerVM.Image.CopyToAsync(stream);
            }

            registerVM.Password = PasswordHash.HashPassword(registerVM.Password);
            var user = _mapper.Map<AppUser>(registerVM);
            user.UserImage = fileName;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();


            return (true, null);
        }


        public async Task<(bool Succeeded, string RedirectAction)> VerifyEmailAsync(string[] codes, HttpContext httpContext)
        {
            string enteredCode = string.Concat(codes);
            string storedCode = httpContext.Session.GetString("email_verification_code");
            string method = httpContext.Session.GetString("method");

            if (storedCode == null || enteredCode != storedCode)
            {
                return (false, null);
            }

            string email = httpContext.Session.GetString("email_to_verify");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (method == "register" && user != null)
            {
                await _context.SaveChangesAsync();
                ClearVerificationSession(httpContext);
                return (true, "Login");
            }
            else if (method == "changePassword")
            {
                
                return (true, "ResetPassword");
            }

            ClearVerificationSession(httpContext);
            return (true, "Login");
        }

        public async Task<(bool Succeeded, string ErrorMessage)> ConfirmEmailAsync(ConfirmEmailVM vm, HttpContext httpContext)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == vm.Email);
            if (!userExists) return (false, "Belə bir e-poçt ünvanı mövcud deyil.");

            var code = new Random().Next(1000, 9999).ToString();
            httpContext.Session.SetString("email_verification_code", code);
            httpContext.Session.SetString("email_to_verify", vm.Email);
            httpContext.Session.SetString("method", "changePassword");

            var body = await _fileService.ReadFileAsync("wwwroot/templates/verify.html");
            body = body.Replace("{{code}}", code);
            _emailService.Send(vm.Email, "Email Verification Code", body);

            return (true, null);
        }

        public async Task<(bool Succeeded, string ErrorMessage)> ResetPasswordAsync(ResetPasswordVM vm, HttpContext httpContext)
        {
            string email = httpContext.Session.GetString("email_to_verify");
            if (string.IsNullOrEmpty(email)) return (false, "Sessiya məlumatı tapılmadı.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return (false, "İstifadəçi tapılmadı.");

            user.Password = PasswordHash.HashPassword(vm.Password);
            await _context.SaveChangesAsync();
            ClearVerificationSession(httpContext);

            return (true, null);
        }

        public async Task ResendVerificationCodeAsync(HttpContext httpContext)
        {
            string email = httpContext.Session.GetString("email_to_verify");
            var code = new Random().Next(1000, 9999).ToString();
            httpContext.Session.SetString("email_verification_code", code);

            var body = await _fileService.ReadFileAsync("wwwroot/templates/verify.html");
            body = body.Replace("{{code}}", code);

            _emailService.Send(email, "Email Verification Code", body);
        }

        public async Task LogoutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private void ClearVerificationSession(HttpContext httpContext)
        {
            httpContext.Session.Remove("email_verification_code");
            httpContext.Session.Remove("email_to_verify");
            httpContext.Session.Remove("method");
        }
        public async Task<IEnumerable<Reservation>> GetMyReservationsAsync(string userId, DateTime selectedDate)
        {
            return await _context.Reservations
                .Where(r => r.BarberId.ToString() == userId && r.ReservationTime.Date == selectedDate.Date)
                .ToListAsync();
        }

    }

}
