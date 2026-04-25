using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BuoiKhamController : Controller
    {
        private readonly IBuoiKhamService _service;

        public BuoiKhamController(IBuoiKhamService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetBacSiVaPhong(int chuyenKhoaId)
        {
            var data = await _service.LayBacSiVaPhongTheoKhoaAsync(chuyenKhoaId);
            return Json(new { success = true, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetGioTrong(int bacSiId, int phongKhamId, string ngay)
        {
            try
            {
                var dateObj = DateOnly.Parse(ngay);
                var gioTrong = await _service.LayCacGioKhamTrongAsync(bacSiId, phongKhamId, dateObj);
                return Json(new { success = true, gioTrong = gioTrong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DatLich(DatLichRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            string role = HttpContext.Session.GetString("UserRole") ?? "BenhNhan";

            if (userId == 0) return RedirectToAction("Login", "Account");

            try
            {
                var result = await _service.DatLichKhamAsync(request, userId, role);
                if (result)
                {
                    TempData["ThongBao"] = "✅ Đặt lịch thành công!";
                }
                else
                {
                    TempData["ThongBao"] = "❌ Lỗi hệ thống, vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ " + ex.Message;
            }

            return RedirectToAction("LichKham", "BenhNhanDashboard");
        }
    }
}