using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Linq;

namespace QuanLyPhongKham.Web.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IBacSiService _bacSiService;
        private readonly IBuoiKhamService _buoiKhamService;
        private readonly IBenhNhanService _benhNhanService;

        // 2. Inject nó vào Constructor
        public AdminDashboardController(
            IBuoiKhamService buoiKhamService,
            IBacSiService bacSiService,
            IBenhNhanService benhNhanService) 
        {
            _buoiKhamService = buoiKhamService;
            _bacSiService = bacSiService;
            _benhNhanService = benhNhanService;
        }

        public async Task<IActionResult> TongQuan()
        {
            var listBacSi = _bacSiService.GetAll();
            ViewBag.TongBacSi = listBacSi != null ? listBacSi.Count() : 0;

            // 2. Thêm chữ 'await' và gọi đúng tên hàm (ví dụ: GetAllAsync) 👇
            var listBenhNhan = await _benhNhanService.GetAllAsync();
            ViewBag.TongBenhNhan = listBenhNhan != null ? listBenhNhan.Count() : 0;

            var listLichKham = _buoiKhamService.GetAllLichKham();
            var homNay = DateOnly.FromDateTime(DateTime.Now);

            ViewBag.LichHomNay = listLichKham != null
                ? listLichKham.Count(l => l.Ngay == homNay)
                : 0;

            return View();
        }

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