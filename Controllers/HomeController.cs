using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WypozyczeniaAPI.Models;

namespace WypozyczeniaAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Zależnie od roli system odsyła użytkownika do stosownego ekranu początkowego 
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "AdminInterface");
            }
            if (User.IsInRole("User"))
            {
                return RedirectToAction("Index", "UzytInterface");
            }
            if (User.IsInRole("Employee"))
            {
                return RedirectToAction("Index", "SerwisInterface");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}