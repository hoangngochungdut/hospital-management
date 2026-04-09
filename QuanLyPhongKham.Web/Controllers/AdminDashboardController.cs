using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models.Enums;

namespace QuanLyPhongKham.Web.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly AppDbContext _context;
        public AdminDashboardController(AppDbContext context) { _context = context; }

        public IActionResult AdminDashboard() 
        {

            return View();
        }

        [HttpGet]
        public IActionResult LichKham()
        {
            // Lấy TẤT CẢ không dùng Where
            // CHỈ CẦN Include BacSi và BenhNhan là xong, XÓA sạch đoạn .ThenInclude(...) đi
            var tatCaLich = _context.BuoiKhams
                .Include(b => b.BacSi)
                .Include(b => b.BenhNhan)
                .Include(b => b.PhongKham)
                .OrderByDescending(b => b.Ngay).ThenBy(b => b.Gio)
                .ToList();

            return View(tatCaLich);
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