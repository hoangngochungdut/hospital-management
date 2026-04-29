using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Web.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IBacSiService _bacSiService;
        private readonly IBuoiKhamService _buoiKhamService;
        private readonly ILeTanService _leTanService;

        public AdminDashboardController(
            IBuoiKhamService buoiKhamService,
            IBacSiService bacSiService,
            ILeTanService leTanService)
        {
            _buoiKhamService = buoiKhamService;
            _bacSiService = bacSiService;
            _leTanService = leTanService;
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        // --- QUẢN LÝ LỊCH KHÁM ---
        [HttpGet]
        public IActionResult LichKham()
        {
            var tatCaLich = _buoiKhamService.GetAllLichKham();
            return View(tatCaLich);
        }

        [HttpPost]
        public IActionResult DoiTrangThai(int id, int trangThaiMoi)
        {
            _buoiKhamService.CapNhatTrangThai(id, (TrangThaiBuoiKham)trangThaiMoi);
            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("LichKham");
        }

        [HttpPost]
        public IActionResult XoaLich(int id)
        {
            bool success = _buoiKhamService.XoaBuoiKham(id);
            if (success)
                TempData["Success"] = "Đã xóa lịch khám thành công!";
            else
                TempData["Error"] = "Lỗi: Không tìm thấy lịch khám này!";

            return RedirectToAction("LichKham");
        }

        // --- QUẢN LÝ BÁC SĨ ---
        [HttpGet]
        public IActionResult BacSi()
        {
            var tatCaBacSi = _bacSiService.GetAll();
            return View(tatCaBacSi);
        }

        // --- QUẢN LÝ LỄ TÂN ---

        // 1. Hiển thị danh sách
        public async Task<IActionResult> LeTan()
        {
            var dsLeTan = await _leTanService.GetAllLeTanAsync();
            return View(dsLeTan);
        }

        // 2. Thêm Lễ Tân
        [HttpPost]
        public async Task<IActionResult> ThemLeTan(LeTan model, string tenDangNhap, string matKhau)
        {
            // Kết quả trả về từ Service là Tuple (bool Success, string Message)
            var result = await _leTanService.CreateLeTanAsync(model, tenDangNhap, matKhau);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("LeTan");
            }

            TempData["Error"] = result.Message;
            return RedirectToAction("LeTan");
        }

        // 3. GET: Trang chỉnh sửa Lễ Tân
        [HttpGet]
        public async Task<IActionResult> SuaLeTan(int id)
        {
            // Fix: Sử dụng GetByIdAsync khớp với ILeTanService
            var leTan = await _leTanService.GetByIdAsync(id);
            if (leTan == null)
            {
                return NotFound();
            }
            return View(leTan);
        }

        // 4. POST: Cập nhật Lễ Tân
        [HttpPost]
        public async Task<IActionResult> SuaLeTan(LeTan model)
        {
            // Fix: Gọi UpdateLeTanAsync với 1 tham số model
            var result = await _leTanService.UpdateLeTanAsync(model);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("LeTan");
            }

            TempData["Error"] = result.Message;
            return View(model);
        }

        // 5. Xóa Lễ Tân
        public async Task<IActionResult> XoaLeTan(int id)
        {
            // Fix: DeleteLeTanAsync giờ trả về Tuple (bool Success, string Message)
            var result = await _leTanService.DeleteLeTanAsync(id);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction("LeTan");
        }
    }
}