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
        // 1. ĐĂNG NHẬP (CÓ CHECK BẮT ĐỔI MẬT KHẨU LẦN ĐẦU)
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

                // 🔥 NẾU TÀI KHOẢN YÊU CẦU ĐỔI MẬT KHẨU (IsMustChangePassword == true)
                // Phản hồi về Frontend để Frontend tự động bật Popup đổi mật khẩu
                if (tk.IsMustChangePassword)
                {
                    return Json(new { success = true, mustChange = true, tkId = tk.Id });
                }

                // Nếu không yêu cầu đổi mật khẩu -> Lưu Session luôn
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

                return Json(new { success = true, mustChange = false, redirectUrl = urlHuong });
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
                    NguoiDung = benhNhan,
                    IsMustChangePassword = false // Đăng ký tự nguyện thì không ép đổi pass
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
        // 3. QUÊN MẬT KHẨU - GỬI TRỰC TIẾP MẬT KHẨU MỚI VÀO MAIL
        // =======================================================
        [HttpPost]
        public async Task<IActionResult> ResetPasswordAjax(string Username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username))
                    return Json(new { success = false, message = "Vui lòng nhập tên đăng nhập!" });

                var tk = _taiKhoanService.GetByUsernameWithNguoiDung(Username);

                if (tk == null)
                    return Json(new { success = false, message = "Tài khoản không tồn tại!" });

                var emailNhan = tk.NguoiDung?.Email;
                if (string.IsNullOrEmpty(emailNhan))
                    return Json(new { success = false, message = "Tài khoản chưa có Email liên kết!" });

                // 1. Tạo mật khẩu mới ngẫu nhiên (Lấy 8 ký tự đầu tiên của Random String)
                string newPassword = _passwordGenerator.Generate(8);

                // 2. Cập nhật thẳng vào Database và BẮT BUỘC ĐỔI PASS Ở LẦN ĐĂNG NHẬP TỚI
                tk.MatKhauHash = newPassword;
                tk.IsMustChangePassword = true; // Yêu cầu đổi pass vì đây là pass hệ thống cấp
                _taiKhoanService.Update(tk);

                // 3. Gửi mật khẩu mới qua SendGrid
                var sendGridSettings = _configuration.GetSection("SendGrid");
                var apiKey = sendGridSettings["ApiKey"];
                var fromEmail = sendGridSettings["FromEmail"];
                var fromName = sendGridSettings["FromName"];

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(emailNhan);
                var subject = "Khôi phục mật khẩu - Hệ thống PBL3";

                var htmlContent = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #e2e8f0; border-radius: 10px; max-width: 500px;'>
                        <h2 style='color: #1e40af;'>Khôi phục mật khẩu</h2>
                        <p>Chào bạn,</p>
                        <p>Hệ thống đã nhận được yêu cầu cấp lại mật khẩu cho tài khoản <strong>{Username}</strong> của bạn.</p>
                        <p>Mật khẩu mới của bạn là: <strong style='font-size: 22px; color: #10b981; background: #ecfdf5; padding: 5px 15px; border-radius: 5px; display: inline-block; margin-top: 10px;'>{newPassword}</strong></p>
                        <p>Hệ thống sẽ yêu cầu bạn đổi mật khẩu ngay trong lần đăng nhập tiếp theo để đảm bảo an toàn.</p>
                        <br/>
                        <p><i>Trân trọng, <br/> Đội ngũ PBL3.</i></p>
                    </div>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Mật khẩu mới đã được gửi vào Email của bạn! Vui lòng kiểm tra hộp thư." });
                }
                return Json(new { success = false, message = "Lỗi gửi mail qua SendGrid: " + response.StatusCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // =========================================================================
        // 4. XỬ LÝ ĐỔI MẬT KHẨU LẦN ĐẦU (AJAX TỪ POPUP ĐỔI MK)
        // =========================================================================
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

        // =========================================================================
        // 5. CÁC HÀM VIEW DỰ PHÒNG & LOGOUT
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