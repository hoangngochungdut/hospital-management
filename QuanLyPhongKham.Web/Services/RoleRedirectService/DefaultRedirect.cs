using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Web.Services.RoleRoutingStrategy;

namespace QuanLyPhongKham.Web.Services.RoleRedirectService
{
    public class DefaultRedirect : IRoleRedirect
    {
        public IActionResult Redirect()
        {
            return new RedirectToActionResult("Login", "Account", null);
        }
    }
}
