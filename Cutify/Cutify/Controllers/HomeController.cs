using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cutify.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

      
    }
}
