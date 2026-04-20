using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class AdminDashboardController : Controller
    {
        // 1. ĐÃ XÓA AppDbContext, CHỈ GIỮ LẠI SERVICE
        private readonly IBacSiService _bacSiService;
        private readonly IBuoiKhamService _buoiKhamService;

        public AdminDashboardController(IBuoiKhamService buoiKhamService, IBacSiService bacSiService)
        {
            _buoiKhamService = buoiKhamService;
            _bacSiService = bacSiService;
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        // 2. HIỂN THỊ TẤT CẢ LỊCH
        [HttpGet]
        public IActionResult LichKham()
        {
            // Lấy toàn bộ lịch từ Service
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

        // 3. THÊM MỚI: TÍNH NĂNG XÓA LỊCH
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

        [HttpGet]
        public IActionResult BacSi()
        {
            var tatCaBacSi = _bacSiService.GetAll();
            return View(tatCaBacSi);
        }

        public IActionResult Edit(int id)
        {
            var bacSi = _bacSiService.GetById(id);
            if (bacSi == null)
            {
                return NotFound();
            }
            return View(bacSi);
        }
    }
}