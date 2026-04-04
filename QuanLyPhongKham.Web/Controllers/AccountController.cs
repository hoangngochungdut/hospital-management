using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Dùng để gọi .Include()
using QuanLyLichKham.Models;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.LowLevelValidators;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories;
using QuanLyPhongKham.Services;
using QuanLyPhongKham.Web.Services.RoleRedirectService;
using System.Data;
using System.Linq;
namespace QuanLyLichKham.Controllers
{
    public class AccountController : Controller
    {
        private readonly TaiKhoanService _tkservice;
        public AccountController(TaiKhoanService service)
        {
            _tkservice = service;
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
            TaiKhoan? tk = _tkservice.GetByUsername(username);

            if (tk != null)
            {
                return RoleRedirectContext.
                GetRoleRedirect(tk.VaiTro).
                Redirect();
            }
            return View();
            
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string fullName)
        {
            PasswordValidator passwordValidator = new PasswordValidator(password);
            if (!passwordValidator.IsValid())
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin (Tên đăng nhập, Mật khẩu, Họ tên)!";
                return View();
            }

            bool isExist = _tkservice.ExistedByUsername(username);
            if (isExist)
            {
                ViewBag.ErrorMessage = "Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác!";
                return View();
            }



            var benhNhan = new BenhNhan
            {
                HoTen = fullName,
                TieuSuBenhAn = new TieuSuBenhAn
                {
                    MoTa = ""
                }
            };

            var newTaiKhoan = new TaiKhoan
            {
                TenDangNhap = username,
                MatKhauHash = password,
                VaiTro = "BN",
                NguoiDung = benhNhan
            };

            _tkservice.Add(newTaiKhoan);

            TempData["SuccessMessage"] = "Tạo tài khoản Bệnh Nhân thành công! Vui lòng đăng nhập để tiếp tục.";
            return RedirectToAction("Login", "Account");
        }
    }
}