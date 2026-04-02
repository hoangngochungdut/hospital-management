using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Web.Services.RoleRoutingStrategy
{
    public interface IRoleRedirect
    {
        IActionResult GetRedirectUrl();
    }
}
