using Microsoft.AspNetCore.Mvc;

namespace Cutify.Controllers
{
    public class ReservationController : Controller
    {
        [HttpGet]
        public IActionResult SelectBarber()
        {
            return View();
        }
        [HttpGet]
        public IActionResult EnterInfo()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SuccessMessage()
        {
            return View();
        }
    }
}
