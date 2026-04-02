using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Dùng để gọi .Include()
using QuanLyLichKham.Models;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
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

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string fullName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin (Tên đăng nhập, Mật khẩu, Họ tên)!";
                return View();
            }

            bool isExist = _context.TaiKhoans.Any(t => t.TenDangNhap == username);
            if (isExist)
            {
                ViewBag.ErrorMessage = "Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác!";
                return View();
            }

            var newNguoiDung = new NguoiDung
            {
                HoTen = fullName

            };

            var newTaiKhoan = new TaiKhoan
            {
                TenDangNhap = username,
                MatKhauHash = password,
                VaiTro = "BN",
                NguoiDung = newNguoiDung
            };

            _context.TaiKhoans.Add(newTaiKhoan);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Tạo tài khoản Bệnh Nhân thành công! Vui lòng đăng nhập để tiếp tục.";
            return RedirectToAction("Login", "Account");
        }
    }
}