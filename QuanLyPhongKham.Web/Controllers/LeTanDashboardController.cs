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

        public async Task<IActionResult> LichKham()
        {
            var dsKhoa = await _buoiKhamService.LayTatCaChuyenKhoaAsync();
            var dsBenhNhan = await _buoiKhamService.LayTatCaBenhNhanAsync();
            ViewBag.DsChuyenKhoa = dsKhoa;
            ViewBag.DsBenhNhan = dsBenhNhan;

            return View();
        }

        [HttpPost]
        // 1. ĐỔI 'TimeOnly Gio' THÀNH 'string Gio' Ở ĐÂY 👇
        public async Task<IActionResult> DatLich(int BenhNhanId, DateOnly Ngay, string Gio, int BacSiId, int PhongKhamId)
        {
            int? currentLeTanId = HttpContext.Session.GetInt32("UserId");
            if (currentLeTanId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. TỰ ÉP KIỂU BẰNG TAY CHO CHẮC CỐP
            TimeOnly gioKhamChuan;
            // Thử ép chuỗi "07:00" thành TimeOnly. Nếu thất bại thì báo lỗi luôn.
            if (!TimeOnly.TryParse(Gio, out gioKhamChuan))
            {
                TempData["ThongBao"] = "❌ Hệ thống không đọc được định dạng giờ (" + Gio + ")!";
                return RedirectToAction("LichKham");
            }

            // Gói dữ liệu vào DTO
            var request = new DatLichRequest
            {
                Ngay = Ngay,
                Gio = gioKhamChuan, // 3. TRUYỀN CÁI GIỜ ĐÃ ÉP KIỂU VÀO ĐÂY 👇
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId,
                BenhNhanId = BenhNhanId
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

            if (result == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("LeTanDashboard");
            }

            return View(result);
        }


        // POST: Cập nhật hồ sơ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoLeTanRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(HoSo));
            }

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
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(HoSo));
            }

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