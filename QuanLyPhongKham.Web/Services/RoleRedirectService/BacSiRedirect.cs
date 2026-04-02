using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Web.Services.RoleRoutingStrategy;

namespace QuanLyPhongKham.Web.Services.RoleRedirectService
{
    public class BacSiRedirect : IRoleRedirect
    {
        public IActionResult GetRedirectUrl()
        {
            return new RedirectToActionResult("BacSiDashboard", "BacSiDashboard", null);
        }
    }
}
