using Microsoft.AspNetCore.Http; // Thêm thư viện này để dùng được Session
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyLichKham.Models;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.LowLevelValidators;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;
using QuanLyPhongKham.Web.Services.RoleRedirectService;
using System.Data;
using System.Linq;

namespace QuanLyLichKham.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly INguoiDungService _nguoiDungService;
        public AccountController(ITaiKhoanService taiKhoanService, INguoiDungService nguoiDungService)
        {
            _taiKhoanService = taiKhoanService;
            _nguoiDungService = nguoiDungService;
        }

        // GET: Hiển thị giao diện đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }
     

        public IActionResult DoiMatKhau()
        {

            //return Content("OK VIEW WORKING");
            return View();
        }

        [HttpPost]
        public IActionResult DoiMatKhau(string MatKhauMoi, string XacNhanMatKhauMoi)
        {
            if (MatKhauMoi != XacNhanMatKhauMoi)
            {
                ViewBag.ErrorMessage = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            var userId = HttpContext.Session.GetInt32("TempUserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var taiKhoanId = HttpContext.Session.GetInt32("TempTaiKhoanId");
            var tk = _taiKhoanService.GetById(taiKhoanId.Value);

            tk.MatKhauHash = MatKhauMoi;
            tk.IsMustChangePassword = false;

            _taiKhoanService.Update(tk);

            HttpContext.Session.Remove("TempUserId");
            HttpContext.Session.Remove("TempTaiKhoanId");

            //return RedirectToAction("Login", "Account");
            HttpContext.Session.SetInt32("UserId", tk.NguoiDungId);
            HttpContext.Session.SetString("Role", tk.VaiTro);

            return tk.VaiTro switch
            {
                "AD" => RedirectToAction("TongQuan", "AdminDashboard"),
                "BS" => RedirectToAction("LichKham", "BacSiDashboard"),
                "BN" => RedirectToAction("LichKham", "BenhNhanDashboard"),
                "LT" => RedirectToAction("LichKham", "LeTanDashboard"),
                _ => RedirectToAction("Login", "Account")
            };
        }


        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            var tk = _taiKhoanService.GetByUsername(username);
            
            if (tk == null || tk.MatKhauHash != password)
            {
                ViewBag.ErrorMessage = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }
            Console.WriteLine("tk.IsMustChangePassword = " + tk.IsMustChangePassword);

            if (tk.IsMustChangePassword == true)
            {
                HttpContext.Session.SetInt32("TempUserId", tk.NguoiDungId);
                HttpContext.Session.SetInt32("TempTaiKhoanId", tk.Id);
                return RedirectToAction("DoiMatKhau", "Account");
            }

            HttpContext.Session.SetInt32("UserId", tk.NguoiDungId);
            HttpContext.Session.SetString("Role", tk.VaiTro);

            var mappedRole = tk.VaiTro switch
            {
                "BS" => "BacSi",
                "BN" => "BenhNhan",
                "LT" => "LeTan",
                "AD" => "Admin",
                _ => tk.VaiTro
            };

            return mappedRole switch
            {
                "Admin" => RedirectToAction("TongQuan", "AdminDashboard"),
                "BacSi" => RedirectToAction("LichKham", "BacSiDashboard"),
                "BenhNhan" => RedirectToAction("XemLichKham", "BenhNhanDashboard"),
                "LeTan" => RedirectToAction("LichKham", "LeTanDashboard"),
                _ => RedirectToAction("Login", "Account")
            };
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterDto entity)
        {
            if (_taiKhoanService.ExistedByUsername(entity.Username))
            {
                ModelState.AddModelError("Username",
                    "Tên đăng nhập này đã tồn tại");
            }

            if (_nguoiDungService.ExistedByEmail(entity.Email))
            {
                ModelState.AddModelError("Email",
                    "Email này đã được sử dụng");
            }

            if (_nguoiDungService.ExistedByPhoneNumber(entity.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber",
                    "Số điện thoại này đã được sử dụng");
            }

            if (!ModelState.IsValid)
            {
                return View(entity);
            }

            var benhNhan = new BenhNhan
            {
                HoTen = entity.FullName,
                Sdt = entity.PhoneNumber,
                Email = entity.Email,
                TieuSuBenhAn = new TieuSuBenhAn
                {
                    MoTa = ""
                }
            };

            var newTaiKhoan = new TaiKhoan
            {
                TenDangNhap = entity.Username,
                MatKhauHash = entity.Password,
                VaiTro = "BN",
                NguoiDung = benhNhan
            };

            _taiKhoanService.Add(newTaiKhoan);

            TempData["SuccessMessage"] = "Tạo tài khoản Bệnh Nhân thành công! Vui lòng đăng nhập để tiếp tục.";
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public IActionResult QuenMatKhau(QuenMatKhauDto entity)
        {
            Console.WriteLine("Di vao QuenMatKhau");
            if (!_taiKhoanService.ExistedByUsername(entity.Username))
            {
                ModelState.AddModelError("Username",
                    "Không tìm thấy tài khoản có tên đăng nhập này.");
            }

            if (!ModelState.IsValid)
            {
                return View(entity);
            }

            var taikhoan = _taiKhoanService.GetByUsername(entity.Username);

            if(taikhoan != null)
            {
                Console.WriteLine("Tim thay tai khoan");
            } else
            {
                Console.WriteLine("Khong tim thay tai khoan");
            }
            Console.WriteLine("Tai khoan id = " + taikhoan.Id);
            
            _taiKhoanService.ResetMatKhau(taikhoan.Id);
            TempData["SuccessMessage"] = "Mật khẩu tạm thời đã được gửi tới email của bạn.";
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}