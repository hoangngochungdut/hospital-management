using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Web.Controllers
{
    public class LeTanDashboardController : Controller
    {
        // trang dành cho Lễ tân (LT)
        public IActionResult LeTanDashboard()
        {
            return View();
        }
        public IActionResult LichKham()
        {
            return View();
        }
    }
}
