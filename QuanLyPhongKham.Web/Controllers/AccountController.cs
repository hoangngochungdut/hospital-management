using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace QuanLyLichKham.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly INguoiDungService _nguoiDungService;
        private readonly IConfiguration _configuration;
        private readonly IPasswordGenerator _passwordGenerator;

        // TIÊM (INJECT) CÁC SERVICE VÀO ĐÂY
        public AccountController(
            ITaiKhoanService taiKhoanService,
            INguoiDungService nguoiDungService,
            IConfiguration configuration,
            IPasswordGenerator passwordGenerator)
        {
            _taiKhoanService = taiKhoanService;
            _nguoiDungService = nguoiDungService;
            _configuration = configuration;
            _passwordGenerator = passwordGenerator;
        }
        [HttpPost]
        public IActionResult LoginAjax(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin!" });

                var tk = _taiKhoanService.GetByUsernameWithNguoiDung(username);

                if (tk == null || tk.MatKhauHash != password)
                    return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });

                if (tk.IsMustChangePassword)
                {
                    return Json(new { success = true, mustChange = true, tkId = tk.Id });
                }

                HttpContext.Session.SetInt32("UserId", tk.NguoiDungId);
                HttpContext.Session.SetString("Role", tk.VaiTro);

                HttpContext.Session.SetString("UserFullName", tk.NguoiDung?.HoTen ?? tk.TenDangNhap);

                var mappedRole = tk.VaiTro switch
                {
                    "BS" => "BacSi",
                    "BN" => "BenhNhan",
                    "LT" => "LeTan",
                    "AD" => "Admin",
                    _ => tk.VaiTro
                };
                var urlHuong = mappedRole switch
                {
                    "Admin" => "/AdminDashboard/TongQuan",
                    "BacSi" => "/BacSiDashboard/LichKham",
                    "BenhNhan" => "/BenhNhanDashboard/XemLichKham",
                    "LeTan" => "/LeTanDashboard/LichKham",
                    _ => "/"
                };

                return Json(new { success = true, mustChange = false, redirectUrl = urlHuong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpPost]
        public IActionResult RegisterAjax(RegisterDto entity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entity.Username) ||
                    string.IsNullOrWhiteSpace(entity.FullName) ||
                    string.IsNullOrWhiteSpace(entity.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(entity.Password))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ các thông tin bắt buộc!" });
                }

                if (_taiKhoanService.ExistedByUsername(entity.Username))
                    return Json(new { success = false, message = "Tên đăng nhập này đã tồn tại!" });

                if (!string.IsNullOrWhiteSpace(entity.Email) && _nguoiDungService.ExistedByEmail(entity.Email))
                    return Json(new { success = false, message = "Email này đã được sử dụng!" });

                if (_nguoiDungService.ExistedByPhoneNumber(entity.PhoneNumber))
                    return Json(new { success = false, message = "Số điện thoại này đã được sử dụng!" });

                var benhNhan = new BenhNhan
                {
                    HoTen = entity.FullName,
                    Sdt = entity.PhoneNumber,
                    Email = entity.Email,
                    TieuSuBenhAn = new TieuSuBenhAn { MoTa = "" }
                };

                var newTaiKhoan = new TaiKhoan
                {
                    TenDangNhap = entity.Username,
                    MatKhauHash = entity.Password,
                    VaiTro = "BN",
                    NguoiDung = benhNhan,
                    IsMustChangePassword = false
                };

                _taiKhoanService.Add(newTaiKhoan);

                var tkDaTao = _taiKhoanService.GetByUsername(entity.Username);
                if (tkDaTao != null)
                {
                    HttpContext.Session.SetInt32("UserId", tkDaTao.NguoiDungId);
                    HttpContext.Session.SetString("Role", tkDaTao.VaiTro);

                    // 🔥 THÊM DÒNG NÀY: Lưu họ tên bệnh nhân vừa đăng ký vào Session toàn cục
                    HttpContext.Session.SetString("UserFullName", entity.FullName);
                }

                return Json(new { success = true, redirectUrl = "/BenhNhanDashboard/XemLichKham" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi đăng ký: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult DoiMatKhauAjax(int TempTaiKhoanId, string MatKhauMoi, string XacNhanMatKhauMoi)
        {
            try
            {
                if (MatKhauMoi != XacNhanMatKhauMoi)
                    return Json(new { success = false, message = "Mật khẩu xác nhận không khớp!" });

                var tk = _taiKhoanService.GetById(TempTaiKhoanId);
                if (tk == null)
                    return Json(new { success = false, message = "Lỗi không tìm thấy tài khoản!" });

                // Cập nhật pass mới và tắt cờ "Yêu cầu đổi pass"
                tk.MatKhauHash = MatKhauMoi;
                tk.IsMustChangePassword = false;
                _taiKhoanService.Update(tk);

                // Lưu Session chính thức sau khi đổi pass thành công
                HttpContext.Session.SetInt32("UserId", tk.NguoiDungId);
                HttpContext.Session.SetString("Role", tk.VaiTro);

                string url = tk.VaiTro switch
                {
                    "BS" => "/BacSiDashboard/LichKham",
                    "BN" => "/BenhNhanDashboard/XemLichKham",
                    "LT" => "/LeTanDashboard/LichKham",
                    "AD" => "/AdminDashboard/TongQuan",
                    _ => "/"
                };
                return Json(new { success = true, redirectUrl = url });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Login() { return View(); }

        [HttpGet]
        public IActionResult Register() { return View(); }

        [HttpGet]
        public IActionResult QuenMatKhau() { return View(); }

        [HttpGet]
        public IActionResult DoiMatKhau() { return View(); }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}