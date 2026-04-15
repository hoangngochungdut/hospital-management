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
        private readonly IBacSiService _bacSiService;
        private readonly IBuoiKhamService _buoiKhamService;
        private readonly AppDbContext _context;
        //public AdminDashboardController(AppDbContext context) { _context = context; }

        public AdminDashboardController(IBuoiKhamService buoiKhamService, AppDbContext context, IBacSiService bacSiService)
        {
            _buoiKhamService = buoiKhamService;
            _context = context;
            _bacSiService = bacSiService;
        }

        public IActionResult AdminDashboard() 
        {

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