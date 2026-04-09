using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models.Enums;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BacSiDashboardController : Controller
    {
        private readonly AppDbContext _context;
        public BacSiDashboardController(AppDbContext context) { _context = context; }

        public IActionResult BacSiDashboard() { return View(); }

        [HttpGet]
        public IActionResult LichKham()
        {
            int? currentBacSiId = HttpContext.Session.GetInt32("UserId");

            // Nếu chưa đăng nhập thì đuổi về
            if (currentBacSiId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // 1. Đặt lại tên biến cho chuẩn
            var lichCuaToi = _context.BuoiKhams
                .Include(b => b.BacSi)
                .Include(b => b.BenhNhan)
                .Include(b => b.PhongKham)
                .Where(b => b.BacSiId == currentBacSiId) // 2. QUAN TRỌNG: Lọc đúng lịch của bác sĩ này
                .OrderByDescending(b => b.Ngay).ThenBy(b => b.Gio)
                .ToList();

            // 3. Trả về đúng tên biến
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
    }
}