using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BuoiKhamController : Controller
    {
        private readonly IBuoiKhamService _service;
        private readonly ILichTrucService _lichTrucService;

        public BuoiKhamController(
            IBuoiKhamService service,
            ILichTrucService lichTrucService)
        {
            _service = service;
            _lichTrucService = lichTrucService;
        }

        // ==================== AJAX: LOAD DATA ====================
        [HttpGet]
        public async Task<IActionResult> GetBacSiVaPhong(int chuyenKhoaId)
        {
            var data = await _service
                .LayBacSiVaPhongTheoKhoaAsync(chuyenKhoaId);

            return Json(new { success = true, data });
        }

        [HttpGet]
        public async Task<IActionResult> GetGioTrong(int bacSiId, int phongKhamId, string ngay)
        {
            try
            {
                var date = DateOnly.Parse(ngay);

                var gioTrong = await _service
                    .LayCacGioKhamTrongAsync(bacSiId, phongKhamId, date);

                return Json(new { success = true, gioTrong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message });
            }
        }

        // ==================== ĐẶT LỊCH ====================
        [HttpPost]
        public async Task<IActionResult> DatLich(DatLichRequest request)
        {
            int userId =
                HttpContext.Session.GetInt32("UserId") ?? 0;

            string role =
                HttpContext.Session.GetString("UserRole") ?? "BenhNhan";

            if (userId == 0)
                return RedirectToAction("Login", "Account");

            try
            {
                var result = await _service
                    .DatLichKhamAsync(request, userId, role);

                TempData["ThongBao"] = result
                    ? "✅ Đặt lịch thành công!"
                    : "❌ Lỗi hệ thống, thử lại.";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ " + ex.Message;
            }

            return RedirectToAction("LichKham", "BenhNhanDashboard");
        }

        // ==================== LỊCH KHẢ DỤNG (THEO LỊCH TRỰC) ====================
        [HttpGet]
        public async Task<IActionResult> GetLichKhamKhaDung(int chuyenKhoaId)
        {
            try
            {
                var data = await _lichTrucService
                    .LayDanhSachLichKhaDungAsync(chuyenKhoaId);

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message });
            }
        }
    }
}