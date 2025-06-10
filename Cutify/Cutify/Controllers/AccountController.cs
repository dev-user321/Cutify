using Cutify.Services.Interface;
using Cutify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Cutify.Data;
using Cutify.Models;
using System.Threading.Tasks;
using Cutify.Repositories;
using Cutify.Repositories.Repository;

namespace Cutify.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly AppDbContext _context;
        private readonly IErrorLogRepository _errorLogRepository;

        public AccountController(IAccountService accountService, AppDbContext context, IErrorLogRepository errorLogRepository)
        {
            _accountService = accountService;
            _context = context;
            _errorLogRepository = errorLogRepository;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            try
            {
                var result = await _accountService.LoginAsync(loginVM, HttpContext);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View(loginVM);
                }

                return RedirectToAction("MyReservations");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Login");
                return StatusCode(500, "Giriş zamanı xəta baş verdi.");
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            try
            {
                var result = await _accountService.RegisterAsync(registerVM, HttpContext);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View(registerVM);
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Register");
                return StatusCode(500, "Qeydiyyat zamanı xəta baş verdi.");
            }
        }

        [HttpGet]
        public IActionResult ConfirmEmail() => View();

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM confirmEmailVM)
        {
            if (!ModelState.IsValid)
                return View(confirmEmailVM);

            try
            {
                var result = await _accountService.ConfirmEmailAsync(confirmEmailVM, HttpContext);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("Email", result.ErrorMessage);
                    return View(confirmEmailVM);
                }

                return RedirectToAction("VerifyEmail");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "ConfirmEmail");
                return StatusCode(500, "Email təsdiqlənməsi zamanı xəta baş verdi.");
            }
        }

        [HttpGet]
        public IActionResult VerifyEmail() => View();

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(string code1, string code2, string code3, string code4)
        {
            try
            {
                var codes = new[] { code1, code2, code3, code4 };
                var result = await _accountService.VerifyEmailAsync(codes, HttpContext);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Daxil edilən kod yanlışdır və ya vaxtı bitmişdir.");
                    return View();
                }

                return RedirectToAction(result.RedirectAction);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "VerifyEmail");
                return StatusCode(500, "Kod yoxlanışı zamanı xəta baş verdi.");
            }
        }

        [HttpGet]
        public IActionResult ResetPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordVM);

            try
            {
                var result = await _accountService.ResetPasswordAsync(resetPasswordVM, HttpContext);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.ErrorMessage);
                    return View(resetPasswordVM);
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "ResetPassword");
                return StatusCode(500, "Şifrə sıfırlanması zamanı xəta baş verdi.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SendAgain()
        {
            try
            {
                await _accountService.ResendVerificationCodeAsync(HttpContext);
                return RedirectToAction("VerifyEmail");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "SendAgain");
                return StatusCode(500, "Kod yenidən göndərilə bilmədi.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _accountService.LogoutAsync(HttpContext);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Logout");
                return StatusCode(500, "Çıxış zamanı xəta baş verdi.");
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyReservations(DateTime? date)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login");

                IQueryable<Reservation> query = _context.Reservations
                    .Where(r => r.BarberId.ToString() == userId);

                if (date.HasValue)
                {
                    var selectedDate = date.Value.Date;
                    query = query.Where(r => r.ReservationTime.Date == selectedDate);
                    ViewBag.SelectedDate = selectedDate;
                }

                var reservations = await query
                    .OrderBy(r => r.ReservationTime)
                    .ToListAsync();

                return View(reservations);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "MyReservations");
                return StatusCode(500, "Rezervasiyalar alınarkən xəta baş verdi.");
            }
        }


    }
}
