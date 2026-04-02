using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Web.Services.RoleRoutingStrategy;
using System.Reflection.Metadata.Ecma335;

namespace QuanLyPhongKham.Web.Services.RoleRedirectService
{
    public class AdminRedirect : IRoleRedirect
    {
        public IActionResult GetRedirectUrl()
        {
            return new RedirectToActionResult("Index", "AdminDashboard", null);
        }
    }
}
