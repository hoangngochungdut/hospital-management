using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Dùng để gọi .Include()
using QuanLyLichKham.Models;
using QuanLyPhongKham.Data;
using System.Linq;

namespace QuanLyLichKham.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Hiển thị giao diện đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Xử lý logic khi người dùng bấm Đăng nhập
        //[HttpPost]
        //public IActionResult Login(string username, string password)
        //{
        //    // 1. Tìm tài khoản và Include luôn dữ liệu NguoiDung tương ứng để lấy trường Role
        //    var user = _context.TaiKhoans
        //        .Include(t => t.NguoiDung)
        //        .FirstOrDefault(t => t.TenDangNhap == username && t.MatKhauHash == password);

        //    if (user != null)
        //    {
        //        // Lấy Role từ bảng NguoiDung
        //        string userRole = user.Nguoidung?.Role?.Trim().ToUpper() ?? "";

        //        // 2. Kiểm tra Role và Điều hướng (Redirect) về đúng trang Dashboard
        //        switch (userRole)
        //        {
        //            case "AD":
        //                TempData["SuccessMessage"] = "Đăng nhập thành công với tư cách Admin!";
        //                return RedirectToAction("AdminDashboard", "Home");

        //            case "LT":
        //                TempData["SuccessMessage"] = "Đăng nhập thành công với tư cách Lễ tân!";
        //                return RedirectToAction("LeTanDashboard", "Home");

        //            case "BS":
        //                TempData["SuccessMessage"] = "Đăng nhập thành công với tư cách Bác sĩ!";
        //                return RedirectToAction("BacSiDashboard", "Home");

        //            default:
        //                // Nếu không khớp role nào hoặc role trống, về trang chủ mặc định
        //                TempData["SuccessMessage"] = "Đăng nhập thành công!";
        //                return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    else
        //    {
        //        // Đăng nhập thất bại: báo lỗi ra View Login
        //        ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
        //        return View();
        //    }
        //}
    }
}