using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.Enums;

namespace QuanLyPhongKham.Web.Controllers
{
    public class LeTanDashboardController : Controller
    {
        private readonly AppDbContext _context;
        public LeTanDashboardController(AppDbContext context) { _context = context; }

        public IActionResult LeTanDashboard() 
        {
            ViewBag.DsBenhNhan = _context.BenhNhans.ToList();
            ViewBag.DsBacSi = _context.BacSis.ToList();
            ViewBag.DsPhongKham = _context.PhongKhams.ToList();
            return View(); 
        }

        [HttpGet]
        public IActionResult LichKham()
        {
            // XÓA cái .Include(b => b.NguoiDung) đi, chỉ cần lấy ToList() là đủ
            ViewBag.DsBacSi = _context.BacSis.ToList();
            ViewBag.DsBenhNhan = _context.BenhNhans.ToList();
            ViewBag.DsPhongKham = _context.PhongKhams.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult DatLich(int BenhNhanId,DateOnly Ngay, TimeOnly Gio, int BacSiId, int PhongKhamId)
        {
            var lichMoi = new BuoiKham
            {
                BenhNhanId = BenhNhanId,
                Ngay = Ngay,
                Gio = Gio,
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId,
                TrangThai = TrangThaiBuoiKham.XacNhan // Lễ tân đặt thì chuyển thẳng sang "Đã xác nhận" (1)
            };

            _context.BuoiKhams.Add(lichMoi);
            _context.SaveChanges();

            TempData["ThongBao"] = "Lễ tân đặt lịch hộ thành công!";
            return RedirectToAction("LichKham");
        }
    }
}