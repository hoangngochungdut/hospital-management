using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Web.Controllers
{
    public class AdminDashboardController : Controller
    {
        // trang dành cho Admin (AD)
        public IActionResult AdminDashboard()
        {
            return View();
        }
        public IActionResult LichKham()
        {
            return View();
        }
    }
}
