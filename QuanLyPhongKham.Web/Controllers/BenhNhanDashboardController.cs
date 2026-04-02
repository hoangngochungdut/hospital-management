using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BenhNhanDashboardController : Controller
    {
        // trang dành cho Bệnh Nhân (BN)
        public IActionResult BenhNhanDashboard()
        {
            return View();
        }
    }
}
