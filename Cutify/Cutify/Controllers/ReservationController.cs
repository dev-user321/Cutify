using AutoMapper;
using Cutify.Data;
using Cutify.Models;
using Cutify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;

namespace Cutify.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ReservationController(AppDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
           
        }
        public async Task<IActionResult> SelectBarber(int pageNumber = 1, int pageSize = 4, string search = "")
        {
            var users = await GetUsersForPaginationAndSearch(pageNumber, pageSize, search);
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> Search(string search)
        {
            var users = await GetUsersForPaginationAndSearch(1, 4, search); 
            return View("SelectBarber", users);
        }

        private async Task<List<AppUser>> GetUsersForPaginationAndSearch(int pageNumber, int pageSize, string search)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => EF.Functions.Like(u.Name, $"%{search}%") || EF.Functions.Like(u.LastName, $"%{search}%"));
            }

            int totalUsers = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = await query
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

           
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchQuery = search;

            return users;
        }



        [HttpGet]
        public IActionResult EnterInfo(int barberId)
        {
            var barber = _context.Users.FirstOrDefault(b => b.Id == barberId);
            if (barber == null)
            {
                return NotFound(); 
            }

            var viewModel = new ReservationVM
            {
                BarberId = barberId,
                BarberFullName = $"{barber.Name} {barber.LastName}", 
            };

            viewModel.FullName = Request.Cookies["FullName"];
            viewModel.PhoneNumber = Request.Cookies["PhoneNumber"];
            return View(viewModel); 
        }


        [HttpPost]
        public async Task<IActionResult> EnterInfo(ReservationVM model)
        {
            if (!ModelState.IsValid)
            {
                var barber = await _context.Users.FirstOrDefaultAsync(b => b.Id == model.BarberId);
                if (barber != null)
                {
                    model.BarberFullName = $"{barber.Name} {barber.LastName}";
                }

                return View(model);
            }

            var reservation = _mapper.Map<Reservation>(model);

            try
            {
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
                var barber = await _context.Users.FirstOrDefaultAsync(b => b.Id == model.BarberId);
                if (barber != null)
                {
                    model.BarberFullName = $"{barber.Name} {barber.LastName}";
                }
                return View(model);
            }

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

        [HttpGet]
        public async Task<IActionResult> GetOccupiedTimes(int barberId, string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                var occupiedTimes = await _context.Reservations
                    .Where(r => r.BarberId == barberId && r.ReservationTime.Date == parsedDate.Date)
                    .Select(r => r.Time)
                    .ToListAsync();

                return Json(occupiedTimes);
            }

            return BadRequest("Düzgün tarix formatı daxil edin.");
        }





        [HttpGet]
        public IActionResult SuccessMessage()
        {
            return View();
        }
    }
}
