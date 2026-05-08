using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BenhNhanController : Controller
    {
        private readonly IBenhNhanService _benhNhanService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IBuoiKhamService _buoiKhamService;
        public BenhNhanController(IBenhNhanService benhNhanService, ITaiKhoanService taiKhoanService, IBuoiKhamService buoiKhamService)
        {
            _benhNhanService = benhNhanService;
            _taiKhoanService = taiKhoanService;
            _buoiKhamService = buoiKhamService;
        }

        [HttpGet]
        public IActionResult LichKham(int id)
        {
            //int? currentId = HttpContext.Session.GetInt32("UserId");

            //if (currentId == null)
                //return RedirectToAction("Login", "Account");

            var lich = _buoiKhamService
                .GetByBenhNhanId(id);

            return View(lich);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddBenhNhanDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                _benhNhanService.Add(model);

                TempData["Success"] = "Thêm bệnh nhân thành công";
                return RedirectToAction("BenhNhan", "AdminDashboard");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var benhNhan = _benhNhanService.GetByIdWithTaiKhoan(id);
            if (benhNhan == null)
            {
                return NotFound();
            }

            CapNhatHoSoBenhNhanRequest capNhatHoSoBenhNhan = new(benhNhan);

            ViewBag.BenhNhanId = id;
            Console.WriteLine("Benh nhan id đang chon = " + ViewBag.BenhNhanId);
            return View(capNhatHoSoBenhNhan);
        }

        [HttpPost]
        public IActionResult Edit(int id, CapNhatHoSoBenhNhanRequest benhnhan)
        {
            if (!ModelState.IsValid)
            {

                return View(benhnhan);
            }

            _benhNhanService.Update(id, benhnhan);

            return RedirectToAction("BenhNhan", "AdminDashboard");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _benhNhanService.Delete(id);
            return RedirectToAction("BenhNhan", "AdminDashboard");
        }

        [HttpPost]
        public IActionResult ResetMatKhau(int benhNhanId)
        {
            //return Content("OK HIT: " + bacSiId);
            var benhnhan = _benhNhanService.GetByIdWithTaiKhoan(benhNhanId);
            if (benhnhan == null)
            {
                Console.WriteLine("Không tìm thấy bệnh nhân");
                return NotFound();
            }

            if (benhnhan.TaiKhoan == null)
            {
                TempData["Error"] = "Bệnh nhân đã xóa";
                return RedirectToAction("BenhNhan", "AdminDashboard");
                Console.WriteLine("Khong tim thay tai khoan cua benh nhan");
            }

            if(string.IsNullOrEmpty(benhnhan.Sdt))
            {
                TempData["Error"] = "Bệnh nhân chưa có số điện thoại, không thể reset mật khẩu được";
                return RedirectToAction("BenhNhan", "AdminDashboard");
                Console.WriteLine("Benh nhan chua co so dien thoai");
            }
            Console.WriteLine("Tai khoan id = " + benhnhan.TaiKhoan.Id);
            _taiKhoanService.ResetMatKhau(benhnhan.TaiKhoan.Id);
            TempData["Success"] = "Reset mật khẩu thành công";
            return RedirectToAction("BenhNhan", "AdminDashboard");
        }
    }
}
