using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Web.Services.RoleRoutingStrategy;

namespace QuanLyPhongKham.Web.Services.RoleRedirectService
{
    public class LeTanRedirect : IRoleRedirect
    {
        public IActionResult Redirect()
        {
            return new RedirectToActionResult("LeTanDashBoard", "LeTanDashboard", null);
        }
    }
}
