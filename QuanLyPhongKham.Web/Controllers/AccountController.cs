using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyLichKham.Models;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
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

        // đăng nhập
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.TaiKhoans
                .Include(t => t.NguoiDung)
                .FirstOrDefault(t => t.TenDangNhap == username && t.MatKhauHash == password);

            if (user != null)
            {
                string userRole = user.VaiTro?.Trim().ToUpper() ?? "";

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
                        return RedirectToAction("BenhNhanDashboard", "Home");

                    default:
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

        // tạo tài khoản

      
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