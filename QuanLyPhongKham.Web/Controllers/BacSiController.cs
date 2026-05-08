using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BacSiController : Controller
    {
        private readonly IBacSiService _bacSiService;
        private readonly IChuyenKhoaService _chuyenKhoaService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IBuoiKhamService _buoiKhamService;
        //private readonly 
        public BacSiController(
            IBacSiService bacSiService, 
            IChuyenKhoaService chuyenKhoaService, 
            ITaiKhoanService taiKhoanService,
            IBuoiKhamService buoiKhamService)
        {
            _bacSiService = bacSiService;
            _chuyenKhoaService = chuyenKhoaService;
            _taiKhoanService = taiKhoanService;
            _buoiKhamService = buoiKhamService;
        }

        [HttpGet]
        public IActionResult ChiTiet(int id)
        {
            var bacSi = _bacSiService.GetByIdWithTaiKhoan(id);
            if (bacSi == null)
            {
                return NotFound();
            }

            CapNhatHoSoBacSiRequest capNhatHoSoBacSi = new(bacSi);

            ViewBag.DanhSachChuyenKhoa = _chuyenKhoaService.GetAll();
            ViewBag.BacSiId = id;
            ViewBag.TenChuyenKhoa = bacSi.ChuyenKhoa?.TenKhoa;
            Console.WriteLine("Bac si id đang chon = " + ViewBag.BacSiId);
            return View(capNhatHoSoBacSi);
        }

        [HttpGet]
        public IActionResult LichKham(int id)
        {
            //int? currentBacSiId = HttpContext.Session.GetInt32("UserId");

            //if (id == null)
                //return RedirectToAction("Login", "Account");

            var lichCuaToi = _buoiKhamService.GetByBacSiId(id);

            return View(lichCuaToi);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var bacSi = _bacSiService.GetByIdWithTaiKhoan(id);
            if (bacSi == null)
            {
                return NotFound();
            }

            CapNhatHoSoBacSiRequest capNhatHoSoBacSi = new(bacSi);

            ViewBag.DanhSachChuyenKhoa = _chuyenKhoaService.GetAll();
            ViewBag.BacSiId = id;
            Console.WriteLine("Bac si id đang chon = " + ViewBag.BacSiId);
            return View(capNhatHoSoBacSi);
        }

        [HttpPost]
        public IActionResult Edit(int id, CapNhatHoSoBacSiRequest bacsi)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DanhSachChuyenKhoa = _chuyenKhoaService.GetAll();

                return View(bacsi);
            }

            _bacSiService.Update(id, bacsi);

            return RedirectToAction("BacSi", "AdminDashboard");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _bacSiService.Delete(id);
            return RedirectToAction("BacSi", "AdminDashboard");
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.DanhSachChuyenKhoa = _chuyenKhoaService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddBacSiDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.DanhSachChuyenKhoa = _chuyenKhoaService.GetAll();
                    return View(model);
                }

                

                _bacSiService.Add(model);

                TempData["Success"] = "Thêm bác sĩ thành công";
                return RedirectToAction("BacSi", "AdminDashboard");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.DanhSachChuyenKhoa = _chuyenKhoaService.GetAll();

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetMatKhau(int bacSiId)
        {
            //return Content("OK HIT: " + bacSiId);
            var bacSi = _bacSiService.GetByIdWithTaiKhoan(bacSiId);
            if (bacSi == null)
            {
                Console.WriteLine("Khong tim thay bac si");
                return NotFound();
            }

            if (bacSi.TaiKhoan == null)
            {
                TempData["Error"] = "Bác sĩ đã nghỉ";
                return RedirectToAction("BacSi", "AdminDashboard");
                Console.WriteLine("Khong tim thay tai khoan cua bac si");
            }
            if (string.IsNullOrEmpty(bacSi.Email))
            {
                TempData["Error"] = "Bác sĩ chưa có email, không thể reset mật khẩu được";
                return RedirectToAction("BacSi", "AdminDashboard");
                Console.WriteLine("Bac si chua co so dien thoai");
            }

            Console.WriteLine("Tai khoan id = " + bacSi.TaiKhoan.Id);
            _taiKhoanService.ResetMatKhau(bacSi.TaiKhoan.Id);
            TempData["Success"] = "Reset mật khẩu thành công";
            return RedirectToAction("BacSi", "AdminDashboard");
        }
    }
}
