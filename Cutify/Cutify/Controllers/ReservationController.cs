using AutoMapper;
using Cutify.Data;
using Cutify.Models;
using Cutify.Repositories;
using Cutify.Repositories.Repository;
using Cutify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Cutify.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IWorkHourRepository _workHourRepository;
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

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimes(int barberId, DateTime selectedDate)
        {
            try
            {
                var reservations = await _reservationRepository.GetReservationsByBarberAndDateAsync(barberId, selectedDate.Date);
                var availableSlots = GetAvailableSlots(reservations, selectedDate)
                    .Select(t => t.ToString(@"hh\:mm"))
                    .ToList();

                return Json(availableSlots);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "GetAvailableTimes");
                return StatusCode(500, "Failed to retrieve available times.");
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
                if (barber == null) return NotFound();

                var selectedDate = DateTime.Today;

                var allReservations = await _context.Reservations
                    .Where(r => r.BarberId == barberId && r.ReservationTime == selectedDate)
                    .ToListAsync();

                var availableSlots = GetAvailableSlots(allReservations, selectedDate)
                    .Select(t => t.ToString(@"hh\:mm"))
                    .ToList();

                var viewModel = new ReservationVM
                {
                    BarberId = barberId,
                    BarberFullName = $"{barber.Name} {barber.LastName}",
                    FullName = Request.Cookies["FullName"],
                    PhoneNumber = Request.Cookies["PhoneNumber"],
                    AllWorkHours = availableSlots,
                    ReservationTime = selectedDate
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "EnterInfo_Get");
                return StatusCode(500, "Xəta baş verdi.");
            }
        }

        public static List<TimeSpan> GetAvailableSlots(List<Reservation> allReservations, DateTime selectedDate)
        {
            TimeSpan startTime = new TimeSpan(9, 0, 0);
            TimeSpan endTime = new TimeSpan(18, 0, 0);
            List<TimeSpan> availableSlots = new();

            var reservedSlots = allReservations
                .Where(r => r.ReservationTime.Date == selectedDate.Date)
                .Select(r => TimeSpan.Parse(r.Time))
                .ToHashSet();

            for (var time = startTime; time < endTime; time = time.Add(TimeSpan.FromHours(1)))
            {
                if (!reservedSlots.Contains(time))
                    availableSlots.Add(time);
            }

            return availableSlots;
        }

        [HttpPost]
        public async Task<IActionResult> EnterInfo(ReservationVM model)
        {
            try
            {
                var barber = await _userRepository.GetByIdAsync(model.BarberId);
                if (!ModelState.IsValid)
                {
                    if (barber != null)
                        model.BarberFullName = $"{barber.Name} {barber.LastName}";
                    return View(model);
                }

                var reservation = _mapper.Map<Reservation>(model);
                await _reservationRepository.AddReservationAsync(reservation);

                WriteReservationCookies(model, barber);
                return RedirectToAction("SuccessMessage");
            }
            catch (Exception ex)
            {
                await _errorLogRepository.LogErrorAsync(ex, "EnterInfo_Post");
                ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);

                var barber = await _userRepository.GetByIdAsync(model.BarberId);
                if (barber != null)
                    model.BarberFullName = $"{barber.Name} {barber.LastName}";

                return View(model);
            }
        }

        private void WriteReservationCookies(ReservationVM model, AppUser barber)
        {
            var expiration = DateTime.Now.AddDays(365);
            var options = new CookieOptions
            {
                Expires = expiration,
                HttpOnly = true,
                Secure = true
            };

            Response.Cookies.Append("FullName", model.FullName, options);
            Response.Cookies.Append("PhoneNumber", model.PhoneNumber, options);
            Response.Cookies.Append("BarberInfo", model.BarberFullName, options);
            Response.Cookies.Append("BarberPhoneNumber", barber.PhoneNumber, options);
            Response.Cookies.Append("Time", model.ReservationTime.ToString("yyyy-MM-dd"), options);
            Response.Cookies.Append("Location", barber.Address ?? "Yoxdur", options);
            Response.Cookies.Append("Duration", model.Time);
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
            ViewBag.FullName = Request.Cookies["FullName"];
            ViewBag.PhoneNumber = Request.Cookies["BarberPhoneNumber"];
            ViewBag.BarberInfo = Request.Cookies["BarberInfo"];
            ViewBag.Time = Request.Cookies["Time"];
            ViewBag.Address = Request.Cookies["Location"];
            ViewBag.Duration = Request.Cookies["Duration"];
            return View();
        }

        [HttpPost]
        public IActionResult ReservationList(DateTime selectedDate)
        {
            return RedirectToAction("MyReservations", "Account", new { date = selectedDate });
        }
    }
}
