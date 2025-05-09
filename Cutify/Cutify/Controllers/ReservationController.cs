using Cutify.Data;
using Cutify.Models;
using Cutify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cutify.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        public ReservationController(AppDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> EnterInfo(int? barberId)
        {
            if (barberId == null)
            {
                return RedirectToAction("SelectBarber");
            }

            var user = await _context.Users.FindAsync(barberId);
            if (user == null)
            {
                return NotFound();
            }

            ReservationVM reservationVM = new ReservationVM
            {
                BarberFullName = user.LastName + " " + user.Name
            };

            return View(reservationVM);
        }

        [HttpGet]
        public IActionResult SuccessMessage()
        {
            return View();
        }
    }
}
