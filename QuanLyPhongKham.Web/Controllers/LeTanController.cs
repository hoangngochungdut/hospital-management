using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Web.Controllers
{
    public class LeTanController : Controller
    {
        private readonly ILeTanService _leTanService;
        private readonly ITaiKhoanService _taiKhoanService;

        public LeTanController(
            ILeTanService leTanService,
            ITaiKhoanService taiKhoanService)
        {
            _leTanService = leTanService;
            _taiKhoanService = taiKhoanService;
        }

        [HttpGet]
        public IActionResult ChiTiet(int id)
        {
            var leTan = _leTanService.GetByIdWithTaiKhoan(id);
            if (leTan == null)
            {
                return NotFound();
            }

            CapNhatHoSoLeTanRequest capNhatHoSoLeTan = new(leTan);

            ViewBag.LeTanId = id;
            return View(capNhatHoSoLeTan);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var leTan = _leTanService.GetByIdWithTaiKhoan(id);
            if (leTan == null)
            {
                return NotFound();
            }

            CapNhatHoSoLeTanRequest capNhatHoSoLeTan = new(leTan);

            ViewBag.LeTanId = id;
            return View(capNhatHoSoLeTan);
        }

        [HttpPost]
        public IActionResult Edit(int id, CapNhatHoSoLeTanRequest letan)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LeTanId = id;
                return View(letan);
            }

            _leTanService.Update(id, letan);

            return RedirectToAction("LeTan", "AdminDashboard");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _leTanService.Delete(id);
            return RedirectToAction("LeTan", "AdminDashboard");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddLeTanDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                _leTanService.Add(model);

                TempData["Success"] = "Thêm lễ tân thành công";
                return RedirectToAction("LeTan", "AdminDashboard");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult ResetMatKhau(int leTanId)
        {
            var leTan = _leTanService.GetByIdWithTaiKhoan(leTanId);
            if (leTan == null)
            {
                return NotFound();
            }

            if (leTan.TaiKhoan == null)
            {
                TempData["Error"] = "Lễ tân đã nghỉ hoặc chưa có tài khoản";
                return RedirectToAction("LeTan", "AdminDashboard");
            }


            _taiKhoanService.ResetMatKhau(leTan.TaiKhoan.Id);
            TempData["Success"] = "Reset mật khẩu thành công";
            return RedirectToAction("LeTan", "AdminDashboard");
        }
    }
}