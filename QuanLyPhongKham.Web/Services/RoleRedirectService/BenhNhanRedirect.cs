using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Web.Services.RoleRoutingStrategy;

namespace QuanLyPhongKham.Web.Services.RoleRedirectService
{
    public class BenhNhanRedirect : IRoleRedirect
    {
        public IActionResult GetRedirectUrl()
        {
            return new RedirectToActionResult("BenhNhanDashboard", "BenhNhanDashboard", null);
        }
    }
}
