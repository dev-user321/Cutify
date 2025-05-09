using Cutify.Services.Interface;
using Cutify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Cutify.Data;
using Cutify.Models;
using System.Threading.Tasks;

namespace Cutify.Controllers
{

    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly AppDbContext _context;

        public AccountController(IAccountService accountService,AppDbContext context)
        {
            _context = context;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var result = await _accountService.LoginAsync(loginVM, HttpContext);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(loginVM);
            }

            return RedirectToAction("MyReservations");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            var result = await _accountService.RegisterAsync(registerVM, HttpContext);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(registerVM);
            }

            return RedirectToAction("VerifyEmail");
        }

        [HttpGet]
        public IActionResult VerifyEmail() => View();

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(string code1, string code2, string code3, string code4)
        {
            var result = await _accountService.VerifyEmailAsync(new[] { code1, code2, code3, code4 }, HttpContext);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Daxil edilən kod yanlışdır və ya vaxtı bitmişdir.");
                return View();
            }

            return RedirectToAction(result.RedirectAction);
        }

        [HttpGet]
        public IActionResult ConfirmEmail() => View();

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM confirmEmailVM)
        {
            if (!ModelState.IsValid)
                return View(confirmEmailVM);

            var result = await _accountService.ConfirmEmailAsync(confirmEmailVM, HttpContext);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("Email", result.ErrorMessage);
                return View(confirmEmailVM);
            }

            return RedirectToAction("VerifyEmail");
        }

        [HttpGet]
        public IActionResult ResetPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordVM);

            var result = await _accountService.ResetPasswordAsync(resetPasswordVM, HttpContext);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(resetPasswordVM);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> SendAgain()
        {
            await _accountService.ResendVerificationCodeAsync(HttpContext);
            return RedirectToAction("VerifyEmail");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync(HttpContext);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        
        public async Task<IActionResult> MyReservations(DateTime? date)
        {
            var email = User.Identity.Name;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email); 
            // Default to current date if no date is provided
            var selectedDate = date?.Date ?? DateTime.Today;

            // Filter reservations by BarberId and selected date
            IEnumerable<Reservation> reservations = _context.Reservations
                .Where(r => r.BarberId.ToString() == user.Id.ToString() && r.ReservationTime.Date == selectedDate)
                .ToList();

            // Pass the selected date to the view for display
            ViewBag.SelectedDate = selectedDate;

            return View(reservations);
        }
    }
}