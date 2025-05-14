using Cutify.Services.Interface;
using Cutify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Cutify.Data;
using Cutify.Models;
using System.Threading.Tasks;
using Cutify.Repositories.Repository;
using Cutify.Repositories;

namespace Cutify.Controllers
{

    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly AppDbContext _context;
        private readonly IErrorLogRepository _errorLogRepository;
        public AccountController(IAccountService accountService,AppDbContext context,IErrorLogRepository errorLogRepository)
        {
            _context = context;
            _accountService = accountService;
            _errorLogRepository = errorLogRepository;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            try
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
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Search");
                return StatusCode(500, "An error occurred while searching for barbers.");
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            try
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
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Search");
                return StatusCode(500, "An error occurred while searching for barbers.");
            }
        }

        [HttpGet]
        public IActionResult VerifyEmail() => View();

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(string code1, string code2, string code3, string code4)
        {
            try
            {
                var result = await _accountService.VerifyEmailAsync(new[] { code1, code2, code3, code4 }, HttpContext);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Daxil edilən kod yanlışdır və ya vaxtı bitmişdir.");
                    return View();
                }

                return RedirectToAction(result.RedirectAction);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Search");
                return StatusCode(500, "An error occurred while searching for barbers.");
            }
        }

        [HttpGet]
        public IActionResult ConfirmEmail() => View();

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM confirmEmailVM)
        {
            try
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
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Search");
                return StatusCode(500, "An error occurred while searching for barbers.");
            }

        }

        [HttpGet]
        public IActionResult ResetPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            try
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
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Search");
                return StatusCode(500, "An error occurred while searching for barbers.");
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
                await _errorLogRepository.LogErrorAsync(ex, "Search");
                return StatusCode(500, "An error occurred while searching for barbers.");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _accountService.LogoutAsync(HttpContext);
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Search");
                return StatusCode(500, "An error occurred while searching for barbers.");
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
                    return RedirectToAction("Login", "Account");

                var selectedDate = date?.Date ?? DateTime.Today;

                var reservations = await _context.Reservations
                    .Where(r => r.BarberId.ToString() == userId && r.ReservationTime.Date == selectedDate)
                    .ToListAsync();

                ViewBag.SelectedDate = selectedDate;

                return View(reservations);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Search");
                return StatusCode(500, "An error occurred while searching for barbers.");
            }
        }

    }
}