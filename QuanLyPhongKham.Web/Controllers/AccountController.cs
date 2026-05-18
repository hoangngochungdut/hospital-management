using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Services.Interfaces;
using System;

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

        // =========================================================================
        // 1. CÁC HÀM XỬ LÝ AJAX TỪ POPUP Ở TRANG CHỦ
        // =========================================================================

        [HttpPost]
        public IActionResult LoginAjax(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin!" });
                }

                var tk = _taiKhoanService.GetByUsername(username);

                if (tk == null || tk.MatKhauHash != password)
                {
                    return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });
                }

                // Nếu tài khoản yêu cầu đổi mật khẩu lần đầu
                if (tk.IsMustChangePassword == true)
                {
                    HttpContext.Session.SetInt32("TempUserId", tk.NguoiDungId);
                    HttpContext.Session.SetInt32("TempTaiKhoanId", tk.Id);
                    return Json(new { success = true, redirectUrl = "/Account/DoiMatKhau" });
                }

                // Lưu Session cho user
                HttpContext.Session.SetInt32("UserId", tk.NguoiDungId);
                HttpContext.Session.SetString("Role", tk.VaiTro);

                // Xác định đường dẫn dựa trên Role
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
        // 2. HÀM ĐĂNG KÝ BẰNG POPUP DÙNG REGISTERDTO (AJAX)
        // =======================================================
        [HttpPost]
        public IActionResult RegisterAjax(RegisterDto entity)
        {
            try
            {
                // 1. Kiểm tra rỗng sơ bộ (Bắt thêm Username)
                if (string.IsNullOrWhiteSpace(entity.Username) ||
                    string.IsNullOrWhiteSpace(entity.FullName) ||
                    string.IsNullOrWhiteSpace(entity.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(entity.Password))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ các thông tin bắt buộc!" });
                }

                // 2. Dùng các hàm Check Validation cũ của ông
                if (_taiKhoanService.ExistedByUsername(entity.Username))
                {
                    return Json(new { success = false, message = "Tên đăng nhập này đã tồn tại!" });
                }

                if (!string.IsNullOrWhiteSpace(entity.Email) && _nguoiDungService.ExistedByEmail(entity.Email))
                {
                    return Json(new { success = false, message = "Email này đã được sử dụng!" });
                }

                if (_nguoiDungService.ExistedByPhoneNumber(entity.PhoneNumber))
                {
                    return Json(new { success = false, message = "Số điện thoại này đã được sử dụng!" });
                }

                // 3. Tạo Object BenhNhan và map dữ liệu Dto vào
                var benhNhan = new BenhNhan
                {
                    HoTen = entity.FullName,
                    Sdt = entity.PhoneNumber,
                    Email = entity.Email,
                    // Nếu bảng BenhNhan của ông có cột DiaChi thì hứng luôn nhé:
                    // DiaChi = entity.DiaChi, 
                    TieuSuBenhAn = new TieuSuBenhAn
                    {
                        MoTa = ""
                    }
                };

                // 4. Tạo Object TaiKhoan
                var newTaiKhoan = new TaiKhoan
                {
                    TenDangNhap = entity.Username,
                    MatKhauHash = entity.Password,
                    VaiTro = "BN",
                    NguoiDung = benhNhan
                };

                // Lưu vào DB
                _taiKhoanService.Add(newTaiKhoan);

                // Lấy ID tự sinh ra để lưu Session Đăng Nhập
                var tkDaTao = _taiKhoanService.GetByUsername(entity.Username);
                if (tkDaTao != null)
                {
                    HttpContext.Session.SetInt32("UserId", tkDaTao.NguoiDungId);
                    HttpContext.Session.SetString("Role", tkDaTao.VaiTro);
                }

                // Chuyển hướng
                return Json(new { success = true, redirectUrl = "/BenhNhanDashboard/XemLichKham" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi đăng ký: " + ex.Message });
            }
        }
        // =======================================================
        // 3. HÀM QUÊN MẬT KHẨU BẰNG POPUP DÙNG DTO (AJAX)
        // =======================================================
        [HttpPost]
        public IActionResult QuenMatKhauAjax(QuenMatKhauDto entity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entity.Username))
                {
                    return Json(new { success = false, message = "Vui lòng nhập tên đăng nhập!" });
                }

                if (!_taiKhoanService.ExistedByUsername(entity.Username))
                {
                    return Json(new { success = false, message = "Không tìm thấy tài khoản có tên đăng nhập này!" });
                }

                var taikhoan = _taiKhoanService.GetByUsername(entity.Username);

                // Gọi hàm của ông (Nó sẽ tự động random pass và gửi mail theo code xịn của ông)
                _taiKhoanService.ResetMatKhau(taikhoan.Id);

                return Json(new { success = true, message = "Mật khẩu mới đã được gửi tới email của bạn. Vui lòng kiểm tra hộp thư!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // =========================================================================
        // 2. CÁC HÀM GET (ĐỂ PHÒNG HỜ NGƯỜI DÙNG GÕ TRỰC TIẾP LÊN URL)
        // =========================================================================

        [HttpGet]
        public IActionResult Login()
        {
            // Nếu dùng giao diện Popup ở trang chủ rồi, hàm này chỉ để dự phòng
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        // =========================================================================
        // 3. CÁC HÀM XỬ LÝ ĐỔI MẬT KHẨU & LOGOUT
        // =========================================================================

        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DoiMatKhauAjax(int TempTaiKhoanId, string MatKhauMoi, string XacNhanMatKhauMoi)
        {
            if (MatKhauMoi != XacNhanMatKhauMoi)
                return Json(new { success = false, message = "Mật khẩu xác nhận không khớp!" });

            var tk = _taiKhoanService.GetById(TempTaiKhoanId);
            if (tk == null)
                return Json(new { success = false, message = "Lỗi không tìm thấy tài khoản!" });

            // Cập nhật pass mới & tắt cờ đổi mật khẩu
            tk.MatKhauHash = MatKhauMoi;
            tk.IsMustChangePassword = false;
            _taiKhoanService.Update(tk);

            // Tự động đăng nhập luôn sau khi đổi pass
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

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home"); // Sửa lại thành chuyển về trang chủ thay vì trang Login
        }
    }
}