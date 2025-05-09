using AutoMapper;
using Cutify.Data;
using Cutify.Models;
using Cutify.Repositories.Repository;
using Cutify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;

namespace Cutify.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public ReservationController(
            IUserRepository userRepository,
            IReservationRepository reservationRepository,
            IErrorLogRepository errorLogRepository,
            IMapper mapper,
            AppDbContext context)
        {
            _userRepository = userRepository;
            _reservationRepository = reservationRepository;
            _errorLogRepository = errorLogRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IActionResult> SelectBarber(int pageNumber = 1, int pageSize = 4, string search = "")
        {
            try
            {
                var users = await _userRepository.GetUsersForPaginationAndSearchAsync(pageNumber, pageSize, search);
                ViewBag.TotalPages = HttpContext.Items["TotalPages"];
                ViewBag.CurrentPage = HttpContext.Items["CurrentPage"];
                ViewBag.SearchQuery = HttpContext.Items["SearchQuery"];
                return View(users);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "SelectBarber");
                return StatusCode(500, "An error occurred while fetching barbers.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search(string search)
        {
            try
            {
                var users = await _userRepository.GetUsersForPaginationAndSearchAsync(1, 4, search);
                ViewBag.TotalPages = HttpContext.Items["TotalPages"];
                ViewBag.CurrentPage = HttpContext.Items["CurrentPage"];
                ViewBag.SearchQuery = HttpContext.Items["SearchQuery"];
                return View("SelectBarber", users);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "Search");
                return StatusCode(500, "An error occurred while searching for barbers.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EnterInfo(int barberId)
        {
            try
            {
                var barber = await _userRepository.GetByIdAsync(barberId);
                if (barber == null)
                {
                    return NotFound();
                }

                var viewModel = new ReservationVM
                {
                    BarberId = barberId,
                    BarberFullName = $"{barber.Name} {barber.LastName}",
                    FullName = Request.Cookies["FullName"],
                    PhoneNumber = Request.Cookies["PhoneNumber"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "EnterInfo_Get");
                return StatusCode(500, "An error occurred while loading the reservation form.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EnterInfo(ReservationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var barber = await _userRepository.GetByIdAsync(model.BarberId);
                    if (barber != null)
                    {
                        model.BarberFullName = $"{barber.Name} {barber.LastName}";
                    }
                    return View(model);
                }

                var reservation = _mapper.Map<Reservation>(model);
                await _reservationRepository.AddReservationAsync(reservation);

                var expirationDate = DateTime.Now.AddDays(365);
                Response.Cookies.Append("FullName", model.FullName, new CookieOptions
                {
                    Expires = expirationDate,
                    HttpOnly = true,
                    Secure = true
                });

                Response.Cookies.Append("PhoneNumber", model.PhoneNumber, new CookieOptions
                {
                    Expires = expirationDate,
                    HttpOnly = true,
                    Secure = true
                });

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "EnterInfo_Post");
                ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
                var barber = await _userRepository.GetByIdAsync(model.BarberId);
                if (barber != null)
                {
                    model.BarberFullName = $"{barber.Name} {barber.LastName}";
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOccupiedTimes(int barberId, string date)
        {
            try
            {
                if (DateTime.TryParse(date, out DateTime parsedDate))
                {
                    var occupiedTimes = await _reservationRepository.GetOccupiedTimesAsync(barberId, parsedDate);
                    return Json(occupiedTimes);
                }

                return BadRequest("Düzgün tarix formatı daxil edin.");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "GetOccupiedTimes");
                return StatusCode(500, "An error occurred while fetching occupied times.");
            }
        }

        [HttpGet]
        public IActionResult SuccessMessage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ReservationList(DateTime selectedDate)
        {
            return RedirectToAction("MyReservations", "Account", new { date = selectedDate });
        }

    }
}
