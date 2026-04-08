using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BacSiDashboardController : Controller
    {
        // trang dành cho Bác sĩ (BS)
        public IActionResult BacSiDashboard()
        {
            return View();
        }
        public IActionResult LichKham()
        {
            return View();
        }
    }
}
