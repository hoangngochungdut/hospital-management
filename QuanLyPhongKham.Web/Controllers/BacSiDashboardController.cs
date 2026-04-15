using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BacSiDashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBacSiService _bacSiService;
        private readonly IBuoiKhamService _buoiKhamService;
       
        public BacSiDashboardController(AppDbContext context, IBacSiService bacSiService, IBuoiKhamService buoiKhamService) {
            _context = context;
            _bacSiService = bacSiService;
            _buoiKhamService = buoiKhamService;
        }

        public IActionResult BacSiDashboard() { return View(); }

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
        public IActionResult DoiTrangThai(int id, int trangThaiMoi)
        {
            var lich = _context.BuoiKhams.Find(id);
            if (lich != null)
            {
                lich.TrangThai = (TrangThaiBuoiKham)trangThaiMoi;
                _context.SaveChanges();
            }
            return RedirectToAction("LichKham");
        }
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = _bacSiService.GetHoSo(userId.Value);
            if (result == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin";
                return RedirectToAction("BacSiDashboard");
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

            var result = _bacSiService.GetHoSo(userId.Value);

            // Lấy danh sách chuyên khoa và phòng khám cho dropdown
            ViewBag.DanhSachChuyenKhoa = _context.ChuyenKhoas.ToList();
            ViewBag.DanhSachPhongKham = _context.PhongKhams.ToList();

            return View(result);
        }

        // POST: Cập nhật hồ sơ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoBacSiRequest request)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, message) = _bacSiService.CapNhatHoSo(userId.Value, request);
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

            var (success, message) = await _bacSiService.DoiMatKhau(userId.Value, request);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }
    }
}