using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class ChuyenKhoaController : Controller
    {
        private readonly IChuyenKhoaService _chuyenKhoaService;

        public ChuyenKhoaController(IChuyenKhoaService chuyenKhoaService)
        {
            _chuyenKhoaService = chuyenKhoaService;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var chuyenKhoa = _chuyenKhoaService.GetById(id);
            if (chuyenKhoa == null) return NotFound();
            return View(chuyenKhoa);
        }

        [HttpPost]
        public IActionResult Edit(ChuyenKhoa entity)
        {
            _chuyenKhoaService.Update(entity);

            return RedirectToAction("ChuyenKhoa", "AdminDashboard");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(ChuyenKhoa entity)
        {
            _chuyenKhoaService.Add(entity);

            return RedirectToAction("ChuyenKhoa", "AdminDashboard");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _chuyenKhoaService.Delete(id);
            return RedirectToAction("ChuyenKhoa", "AdminDashboard");
        }

    }
}
