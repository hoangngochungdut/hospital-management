using QuanLyPhongKham.Web.Services.RoleRoutingStrategy;

namespace QuanLyPhongKham.Web.Services.RoleRedirectService
{
    public class RoleRedirectContext
    {
        public static IRoleRedirect GetRoleRedirect(string role)
        {
            return role switch
            {
                "AD" => new AdminRedirect(),
                "BN" => new BenhNhanRedirect(),
                "BS" => new BacSiRedirect(),
                "LT" => new LeTanRedirect(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
