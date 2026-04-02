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
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // 1. Tìm tài khoản và Include bảng NguoiDung để lấy Role
            // Lưu ý: Tên bảng/biến phải khớp chính xác với Model bạn đã tạo (NguoiDung hay Nguoidung)
            var user = _context.TaiKhoans
                .Include(t => t.NguoiDung)
                .FirstOrDefault(t => t.TenDangNhap == username && t.MatKhauHash == password);

            if (user != null)
            {
                // Lấy Role và xử lý chuỗi để tránh lỗi so sánh
                // Dùng dấu ? để tránh lỗi NullReferenceException nếu NguoiDung bị null
                string userRole = user.VaiTro?.Trim().ToUpper() ?? "";

                // 2. Điều hướng dựa trên Role
                switch (userRole)
                {
                    case "AD":
                        TempData["SuccessMessage"] = "Đăng nhập thành công với tư cách Admin!";
                        return RedirectToAction("AdminDashboard", "Home");

                    case "LT":
                        TempData["SuccessMessage"] = "Đăng nhập thành công với tư cách Lễ tân!";
                        return RedirectToAction("LeTanDashboard", "Home");

                    case "BS":
                        TempData["SuccessMessage"] = "Đăng nhập thành công với tư cách Bác sĩ!";
                        return RedirectToAction("BacSiDashboard", "Home");

                    case "BN":
                        TempData["SuccessMessage"] = "Chào mừng Bệnh nhân quay lại!";
                        return RedirectToAction("Index", "Home");

                    default:
                        // Thêm dấu gạch chéo // cho comment ở đây để không bị lỗi build
                        TempData["SuccessMessage"] = "Đăng nhập thành công!";
                        return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }
        }
    }
}