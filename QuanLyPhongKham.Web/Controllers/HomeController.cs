using Microsoft.AspNetCore.Mvc;
using QuanLyLichKham.Models;
using System.Diagnostics;

namespace QuanLyLichKham.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Trang chủ dành cho khách hàng
        public IActionResult Index()
        {
            return View();
        }

        // Trang Dashboard dành cho Admin (AD)
        public IActionResult AdminDashboard()
        {
            return View();
        }

        // Trang Dashboard dành cho Bác sĩ (BS)
        public IActionResult BacSiDashboard()
        {
            return View();
        }

        // Trang Dashboard dành cho Lễ tân (LT)
        public IActionResult LeTanDashboard()
        {
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