using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.Enums;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BenhNhanDashboardController : Controller
    {
        private readonly AppDbContext _context;
        public BenhNhanDashboardController(AppDbContext context) { _context = context; }

        public IActionResult BenhNhanDashboard() 
        {
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
        public IActionResult DatLich(DateOnly Ngay, TimeOnly Gio, int BacSiId, int PhongKhamId)
        {
            // Giả lập ID bệnh nhân là 1 (Thực tế lấy từ User đang đăng nhập)
            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var lichMoi = new BuoiKham
            {
                Ngay = Ngay, // Bây giờ DateOnly truyền cho DateOnly -> HẾT ĐỎ
                Gio = Gio,   // TimeOnly truyền cho TimeOnly -> HẾT ĐỎ
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId,
                BenhNhanId = currentBenhNhanId.Value,
                TrangThai = TrangThaiBuoiKham.ChuaXacNhan
            };
            _context.BuoiKhams.Add(lichMoi);
            _context.SaveChanges();

            TempData["ThongBao"] = "Đặt lịch thành công!";
            return RedirectToAction("LichKham");
        }
    }
}