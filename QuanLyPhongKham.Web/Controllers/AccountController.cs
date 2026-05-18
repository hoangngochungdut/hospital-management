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

        // =========================================================================
        // 1. ĐĂNG NHẬP (CÓ BẮT ĐỔI MẬT KHẨU LẦN ĐẦU TRỰC TIẾP TRÊN POPUP)
        // =========================================================================
        // =========================================================================
        // 1. ĐĂNG NHẬP AJAX (Đã bỏ bắt buộc đổi mật khẩu)
        // =========================================================================
        [HttpPost]
        public IActionResult LoginAjax(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin!" });

                var tk = _taiKhoanService.GetByUsername(username);

                if (tk == null || tk.MatKhauHash != password)
                    return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });

                // Đăng nhập thành công -> Lưu Session luôn
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

                var urlHuong = mappedRole switch
                {
                    "Admin" => "/AdminDashboard/TongQuan",
                    "BacSi" => "/BacSiDashboard/LichKham",
                    "BenhNhan" => "/BenhNhanDashboard/XemLichKham",
                    "LeTan" => "/LeTanDashboard/LichKham",
                    _ => "/"
                };

                return Json(new { success = true, redirectUrl = urlHuong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // =======================================================
        // 2. ĐĂNG KÝ AJAX
        // =======================================================
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
                    NguoiDung = benhNhan
                };

                _taiKhoanService.Add(newTaiKhoan);

                var tkDaTao = _taiKhoanService.GetByUsername(entity.Username);
                if (tkDaTao != null)
                {
                    HttpContext.Session.SetInt32("UserId", tkDaTao.NguoiDungId);
                    HttpContext.Session.SetString("Role", tkDaTao.VaiTro);
                }

                return Json(new { success = true, redirectUrl = "/BenhNhanDashboard/XemLichKham" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi đăng ký: " + ex.Message });
            }
        }
        // =======================================================
        // 3. QUÊN MẬT KHẨU - BƯỚC 1: TẠO VÀ GỬI OTP BẰNG SENDGRID
        // =======================================================
        [HttpPost]
        public async Task<IActionResult> SendOtpAjax(string Username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username))
                    return Json(new { success = false, message = "Vui lòng nhập tên đăng nhập!" });

                // 🔥 SỬA ĐÚNG DÒNG NÀY: Gọi hàm mới có chữ "WithNguoiDung"
                var tk = _taiKhoanService.GetByUsernameWithNguoiDung(Username);

                if (tk == null)
                    return Json(new { success = false, message = "Tài khoản không tồn tại!" });

                // Nhờ hàm mới, chỗ này chắc chắn lấy được Email
                var emailNhan = tk.NguoiDung?.Email;
                if (string.IsNullOrEmpty(emailNhan))
                    return Json(new { success = false, message = "Tài khoản chưa có Email liên kết!" });

                // 1. Tạo OTP 6 số bằng class bảo mật của ông
                string otp = _passwordGenerator.GenerateNumericOtp(6);

                // Lưu tạm vào Session
                HttpContext.Session.SetString("OTP_" + Username, otp);

                // 2. Cấu hình SendGrid
                var sendGridSettings = _configuration.GetSection("SendGrid");
                var apiKey = sendGridSettings["ApiKey"];
                var fromEmail = sendGridSettings["FromEmail"];
                var fromName = sendGridSettings["FromName"];

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(emailNhan);
                var subject = "Mã OTP Khôi phục mật khẩu - Hệ thống PBL3";

                var htmlContent = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Khôi phục mật khẩu</h2>
                        <p>Chào bạn, bạn vừa yêu cầu khôi phục mật khẩu cho tài khoản <strong>{Username}</strong>.</p>
                        <p>Mã OTP xác nhận của bạn là: <strong style='font-size: 24px; color: #f97316; letter-spacing: 5px;'>{otp}</strong></p>
                        <p>Vui lòng nhập mã này vào trang web để tự đặt lại mật khẩu mới.</p>
                        <p><i>Lưu ý: Không chia sẻ mã OTP này cho bất kỳ ai.</i></p>
                    </div>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Mã OTP đã được gửi vào Email của bạn!" });
                }
                return Json(new { success = false, message = "Lỗi gửi mail qua SendGrid: " + response.StatusCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpPost]
        public IActionResult VerifyOtpAjax(string Username, string Otp)
        {
            try
            {
                string savedOtp = HttpContext.Session.GetString("OTP_" + Username);
                if (string.IsNullOrEmpty(savedOtp))
                    return Json(new { success = false, message = "Mã OTP đã hết hạn hoặc không tồn tại!" });

                if (savedOtp != Otp)
                    return Json(new { success = false, message = "Mã OTP không chính xác!" });

                // Nếu đúng, trả về true để giao diện hiện Form đổi mật khẩu
                return Json(new { success = true, message = "Xác nhận mã OTP thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // =======================================================
        // 4. QUÊN MẬT KHẨU - BƯỚC 2: XÁC NHẬN OTP & ĐỔI PASS
        // =======================================================
        [HttpPost]
        public IActionResult ResetPassWithOtpAjax(string Username, string Otp, string NewPassword)
        {
            try
            {
                string savedOtp = HttpContext.Session.GetString("OTP_" + Username);

                if (string.IsNullOrEmpty(savedOtp))
                    return Json(new { success = false, message = "Mã OTP đã hết hạn hoặc không tồn tại!" });

                if (savedOtp != Otp)
                    return Json(new { success = false, message = "Mã OTP không chính xác!" });

                // Cập nhật mật khẩu mới do người dùng nhập
                var tk = _taiKhoanService.GetByUsername(Username);
                tk.MatKhauHash = NewPassword;
                _taiKhoanService.Update(tk);

                // Xóa OTP khỏi session
                HttpContext.Session.Remove("OTP_" + Username);

                return Json(new { success = true, message = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập lại." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // =========================================================================
        // 5. ĐỔI MẬT KHẨU LẦN ĐẦU (AJAX TỪ POPUP)
        // =========================================================================
        [HttpPost]
        public IActionResult DoiMatKhauAjax(int TempTaiKhoanId, string MatKhauMoi, string XacNhanMatKhauMoi)
        {
            if (MatKhauMoi != XacNhanMatKhauMoi)
                return Json(new { success = false, message = "Mật khẩu xác nhận không khớp!" });

            var tk = _taiKhoanService.GetById(TempTaiKhoanId);
            if (tk == null)
                return Json(new { success = false, message = "Lỗi không tìm thấy tài khoản!" });

            tk.MatKhauHash = MatKhauMoi;
            tk.IsMustChangePassword = false;
            _taiKhoanService.Update(tk);

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

        // =========================================================================
        // 6. CÁC HÀM VIEW DỰ PHÒNG & LOGOUT
        // =========================================================================
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