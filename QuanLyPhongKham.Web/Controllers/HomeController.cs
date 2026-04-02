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

        
        public IActionResult Index()
        {
            return View();
        }

        // trang dành cho Admin (AD)
        public IActionResult AdminDashboard()
        {
            return View();
        }

        // trang dành cho Bác sĩ (BS)
        public IActionResult BacSiDashboard()
        {
            return View();
        }

        // trang dành cho Lễ tân (LT)
        public IActionResult LeTanDashboard()
        {
            return View();
        }
        // trang dành cho Bệnh Nhân (BN)
        public IActionResult BenhNhanDashboard()
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