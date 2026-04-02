using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Dùng để gọi .Include()
using QuanLyLichKham.Models;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Web.Services.RoleRedirectService;
using System.Linq;

namespace QuanLyLichKham.Controllers
{
    public class DangNhapController : Controller
    {
        private readonly AppDbContext _context;

        public DangNhapController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Hiển thị giao diện đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // 1. Tìm tài khoản và Include bảng NguoiDung để lấy Role
            // Lưu ý: Tên bảng/biến phải khớp chính xác với Model bạn đã tạo (NguoiDung hay Nguoidung)
            var user = _context.TaiKhoans
                .FirstOrDefault(t => t.TenDangNhap == username && t.MatKhauHash == password);

            if (user == null)
            {
                return View();
            }

            string role = user.VaiTro?.Trim().ToUpper() ?? "";

            return RoleRedirectContext.
                GetRoleRedirect(role).
                GetRedirectUrl();
        }
    }
}