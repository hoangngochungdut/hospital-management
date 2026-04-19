using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class LeTanDashboardController : Controller
    {
        private readonly ILeTanService _leTanService;
        private readonly IBuoiKhamService _buoiKhamService;

        public LeTanDashboardController(
            ILeTanService leTanService,
            IBuoiKhamService buoiKhamService)
        {
            _leTanService = leTanService;
            _buoiKhamService = buoiKhamService;
        }

        public IActionResult LeTanDashboard()
        {
            return View();
        }

        public async Task<IActionResult> LichKham()
        {
            var dsKhoa = await _buoiKhamService.LayTatCaChuyenKhoaAsync();
            var dsBenhNhan = await _buoiKhamService.LayTatCaBenhNhanAsync();
            ViewBag.DsChuyenKhoa = dsKhoa;
            ViewBag.DsBenhNhan = dsBenhNhan;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DatLich(int BenhNhanId, DateOnly Ngay, TimeOnly Gio, int BacSiId, int PhongKhamId)
        {
            int? currentLeTanId = HttpContext.Session.GetInt32("UserId");
            if (currentLeTanId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Gói dữ liệu vào DTO
            var request = new DatLichRequest
            {
                Ngay = Ngay,
                Gio = Gio,
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId,
                BenhNhanId = BenhNhanId // Bắt buộc phải có vì Lễ tân đang đặt hộ
            };

            try
            {
                // Gọi Service. Truyền role "LeTan" xuống để Service tự động set trạng thái thành "Xác nhận"
                var result = await _buoiKhamService.DatLichKhamAsync(request, currentLeTanId.Value, "LeTan");

                if (result)
                {
                    TempData["ThongBao"] = "✅ Lễ tân đặt lịch hộ thành công (Đã tự động xác nhận)!";
                }
            }
            catch (Exception ex)
            {
                // Nếu trùng lịch/trùng phòng, Service sẽ ném lỗi ra đây
                TempData["ThongBao"] = ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        [HttpGet]
        public async Task<IActionResult> GetBacSiVaPhong(int chuyenKhoaId)
        {
            try
            {
                // Gọi Service lấy data (Hàm này ông đã viết ở BuoiKhamService rồi)
                var data = await _buoiKhamService.LayBacSiVaPhongTheoKhoaAsync(chuyenKhoaId);

                // Ném cục data đó ra dưới dạng văn bản JSON
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGioTrong(int bacSiId, int phongKhamId, string ngay)
        {
            try
            {
                var dateObj = DateOnly.Parse(ngay);
                var gioTrong = await _buoiKhamService.LayCacGioKhamTrongAsync(bacSiId, phongKhamId, dateObj);

                return Json(new { success = true, gioTrong = gioTrong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Thông tin cá nhân (xem)
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = _leTanService.GetHoSo(userId.Value);
            if (result == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin";
                return RedirectToAction("LeTanDashboard");
            }

            return View(result);
        }

        // GET: Chỉnh sửa hồ sơ
        [HttpGet]
        public IActionResult HoSo()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = _leTanService.GetHoSo(userId.Value);
            return View(result);
        }

        // POST: Cập nhật hồ sơ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoLeTanRequest request)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, message) = _leTanService.CapNhatHoSo(userId.Value, request);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(ThongTinCaNhan));
        }

        // POST: Đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauRequest request)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (request.MatKhauMoi != request.XacNhanMatKhauMoi)
            {
                TempData["Error"] = "Mật khẩu mới và xác nhận không khớp";
                return RedirectToAction(nameof(HoSo));
            }

            var (success, message) = await _leTanService.DoiMatKhau(userId.Value, request);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }
    }
}