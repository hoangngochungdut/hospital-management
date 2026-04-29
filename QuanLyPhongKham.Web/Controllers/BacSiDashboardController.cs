using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BacSiDashboardController : Controller
    {
        private readonly IBacSiService _bacSiService;
        private readonly IBuoiKhamService _buoiKhamService;
        private readonly IBenhNhanService _benhNhanService;

        public BacSiDashboardController(
            IBacSiService bacSiService,
            IBuoiKhamService buoiKhamService,
            IBenhNhanService benhNhanService)
        {
            _bacSiService = bacSiService;
            _buoiKhamService = buoiKhamService;
            _benhNhanService = benhNhanService;
        }

        // ==================== LỊCH KHÁM ====================
        [HttpGet]
        public IActionResult LichKham()
        {
            int? currentBacSiId = HttpContext.Session.GetInt32("UserId");

            if (currentBacSiId == null)
                return RedirectToAction("Login", "Account");

            var lichCuaToi = _buoiKhamService.GetByBacSiId(currentBacSiId.Value);

            return View(lichCuaToi);
        }

        [HttpPost]
        public IActionResult DoiTrangThai(int id, int trangThaiMoi, string? ghiChu)
        {
            try
            {
                _buoiKhamService.CapNhatTrangThai(
                    id,
                    (TrangThaiBuoiKham)trangThaiMoi,
                    ghiChu
                );

                TempData["ThongBao"] = "✅ Cập nhật trạng thái thành công!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Cập nhật thất bại: " + ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        // Dời lịch khám
        [HttpPost]
        public IActionResult DoiLichKham(int id, DateTime ngayMoi, string gioMoi, string lyDoDoi)
        {
            try
            {
                DateOnly dateParsed = DateOnly.FromDateTime(ngayMoi);
                TimeOnly timeParsed = TimeOnly.Parse(gioMoi);

                _buoiKhamService.DoiLichKham(id, dateParsed, timeParsed, lyDoDoi);

                TempData["ThongBao"] =
                    "✅ Dời lịch thành công! Bệnh nhân đã nhận được thông báo.";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] =
                    "❌ Không thể dời lịch: " + ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        // ==================== HỒ SƠ ====================
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var result = _bacSiService.GetHoSo(userId.Value);

            if (result == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin";
                return RedirectToAction("BacSiDashboard");
            }

            return View(result);
        }

        [HttpGet]
        public IActionResult HoSo()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var result = _bacSiService.GetHoSo(userId.Value);

            ViewBag.DanhSachChuyenKhoa =
                _bacSiService.GetDanhSachChuyenKhoa();

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoBacSiRequest request)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ!";
                return RedirectToAction(nameof(HoSo));
            }

            var (success, message) =
                _bacSiService.CapNhatHoSo(userId.Value, request);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(ThongTinCaNhan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauRequest request)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (request.MatKhauMoi != request.XacNhanMatKhauMoi)
            {
                TempData["Error"] = "Mật khẩu mới và xác nhận không khớp";
                return RedirectToAction(nameof(HoSo));
            }

            var (success, message) =
                await _bacSiService.DoiMatKhau(userId.Value, request);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }
        //public IActionResult TieuSuBenhNhan(int id)
        //{
        //    var data = _benhNhanService.GetTieuSu(id);
        //    return PartialView("TieuSuBenhNhan", data);
        //}
        public IActionResult TieuSuBenhNhan(int id)
        {
            try
            {
                var data = _benhNhanService.GetTieuSu(id);

                if (data == null)
                    return Content("DATA NULL");

                return PartialView(data);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString()); // QUAN TRỌNG
            }
        }
    }
}