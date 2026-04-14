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
        private readonly AppDbContext _context;
        private readonly ILeTanService _leTanService;

        public LeTanDashboardController(AppDbContext context, ILeTanService leTanService)
        {
            _context = context;
            _leTanService = leTanService;
        }

        //   public IActionResult LeTanDashboard()
        //{
        //    ViewBag.DsBenhNhan = _context.BenhNhans.ToList();
        //    ViewBag.DsBacSi = _context.BacSis.ToList();
        //    ViewBag.DsPhongKham = _context.PhongKhams.ToList();
        //    return View();
        //}
        public IActionResult LeTanDashboard()
        {
            return View();
        }


        [HttpGet]
        public IActionResult LichKham()
        {
            ViewBag.DsBacSi = _context.BacSis.ToList();
            ViewBag.DsBenhNhan = _context.BenhNhans.ToList();
            ViewBag.DsPhongKham = _context.PhongKhams.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult DatLich(int BenhNhanId, DateOnly Ngay, TimeOnly Gio, int BacSiId, int PhongKhamId)
        {
            var lichMoi = new BuoiKham
            {
                BenhNhanId = BenhNhanId,
                Ngay = Ngay,
                Gio = Gio,
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId,
                TrangThai = TrangThaiBuoiKham.XacNhan
            };

            _context.BuoiKhams.Add(lichMoi);
            _context.SaveChanges();

            TempData["ThongBao"] = "Lễ tân đặt lịch hộ thành công!";
            return RedirectToAction("LichKham");
        }

        // ==================== CODE MỚI ====================

        // ❌ ĐÃ XÓA action Index (không cần)

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