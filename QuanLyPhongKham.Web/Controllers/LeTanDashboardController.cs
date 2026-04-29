using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // ==========================================
        // 1. DASHBOARD & THANH TOÁN
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> LeTanDashboard()
        {
            var dsChoThanhToan = await _leTanService.GetDanhSachChoThanhToanAsync();
            return View(dsChoThanhToan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanThanhToan(int buoiKhamId, decimal tongTien)
        {
            try
            {
                var result = await _leTanService.XacNhanThanhToanAsync(buoiKhamId, tongTien);
                if (result) TempData["ThongBao"] = "✅ Đã xác nhận thanh toán thành công!";
                else TempData["ThongBao"] = "❌ Lỗi: Không thể xử lý thanh toán.";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi hệ thống: " + ex.Message;
            }
            return RedirectToAction(nameof(LeTanDashboard));
        }

        // ==========================================
        // 2. QUẢN LÝ ĐẶT LỊCH HỘ
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> LichKham()
        {
            ViewBag.DsChuyenKhoa = await _buoiKhamService.LayTatCaChuyenKhoaAsync();
            ViewBag.DsBenhNhan = await _buoiKhamService.LayTatCaBenhNhanAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatLich(int BenhNhanId, DateOnly Ngay, string Gio, int BacSiId, int PhongKhamId)
        {
            int? currentLeTanId = HttpContext.Session.GetInt32("UserId");
            if (currentLeTanId == null) return RedirectToAction("Login", "Account");

            if (!TimeOnly.TryParse(Gio, out TimeOnly gioKhamChuan))
            {
                TempData["ThongBao"] = "❌ Định dạng giờ không hợp lệ!";
                return RedirectToAction(nameof(LichKham));
            }

            var request = new DatLichRequest
            {
                Ngay = Ngay,
                Gio = gioKhamChuan,
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId,
                BenhNhanId = BenhNhanId
            };

            try
            {
                var result = await _buoiKhamService.DatLichKhamAsync(request, currentLeTanId.Value, "LeTan");
                if (result)
                {
                    TempData["ThongBao"] = "✅ Đặt lịch thành công!";
                    return RedirectToAction(nameof(LeTanDashboard));
                }
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ " + ex.Message;
            }

            return RedirectToAction(nameof(LichKham));
        }

        // ==========================================
        // 3. QUẢN LÝ HỒ SƠ BỆNH NHÂN (MỤC 6)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> QuanLyBenhNhan(string keyword)
        {
            // ĐÃ SỬA: Gọi qua _leTanService thay vì _context
            var records = await _leTanService.TimKiemBenhNhanAsync(keyword ?? "");

            ViewBag.Keyword = keyword;
            return View(records);
        }

        [HttpGet]
        public async Task<IActionResult> LichSuBenhNhan(int id)
        {
            var detail = await _leTanService.GetChiTietBenhNhanAsync(id);
            if (detail == null)
            {
                TempData["ThongBao"] = "❌ Không tìm thấy hồ sơ bệnh nhân!";
                return RedirectToAction(nameof(QuanLyBenhNhan));
            }
            return View(detail);
        }

       
        [HttpGet]
        public async Task<IActionResult> GetBacSiVaPhong(int chuyenKhoaId)
        {
            var data = await _buoiKhamService.LayBacSiVaPhongTheoKhoaAsync(chuyenKhoaId);
            return Json(new { success = true, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetGioTrong(int bacSiId, int phongKhamId, string ngay)
        {
            if (DateOnly.TryParse(ngay, out DateOnly dateObj))
            {
                var gioTrong = await _buoiKhamService.LayCacGioKhamTrongAsync(bacSiId, phongKhamId, dateObj);
                return Json(new { success = true, gioTrong = gioTrong });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var result = _leTanService.GetHoSo(userId.Value);
            return result == null ? RedirectToAction(nameof(LeTanDashboard)) : View(result);
        }
    }
}