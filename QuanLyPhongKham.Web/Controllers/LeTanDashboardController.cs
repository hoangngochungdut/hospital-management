using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class LeTanDashboardController : Controller
    {
        private readonly ILeTanService _leTanService;
        private readonly IBuoiKhamService _buoiKhamService;
        private readonly IBenhNhanService _benhNhanService;

        public LeTanDashboardController(
            ILeTanService leTanService,
            IBuoiKhamService buoiKhamService,
            IBenhNhanService benhNhanService)
        {
            _leTanService = leTanService;
            _buoiKhamService = buoiKhamService;
            _benhNhanService = benhNhanService;
        }

        // ==================== ĐẶT LỊCH ====================
        [HttpGet]
        public async Task<IActionResult> LichKham()
        {
            ViewBag.DsChuyenKhoa =
                await _buoiKhamService.LayTatCaChuyenKhoaAsync();

            ViewBag.DsBenhNhan =
                await _benhNhanService.GetAllAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DatLich(
            int BenhNhanId,
            DateOnly Ngay,
            string Gio,
            int BacSiId,
            int PhongKhamId)
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");

            if (currentId == null)
                return RedirectToAction("Login", "Account");

            if (!TimeOnly.TryParse(Gio, out var gioParsed))
            {
                TempData["ThongBao"] =
                    $"❌ Không đọc được giờ ({Gio})!";
                return RedirectToAction("LichKham");
            }

            var request = new DatLichRequest
            {
                Ngay = Ngay,
                Gio = gioParsed,
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId,
                BenhNhanId = BenhNhanId
            };

            try
            {
                var result = await _buoiKhamService
                    .DatLichKhamAsync(request, currentId.Value, "LeTan");

                if (result)
                    TempData["ThongBao"] =
                        "✅ Đặt lịch hộ thành công (đã xác nhận)!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        // ==================== AJAX ====================
        [HttpGet]
        public async Task<IActionResult> GetBacSiVaPhong(int chuyenKhoaId)
        {
            try
            {
                var data = await _buoiKhamService
                    .LayBacSiVaPhongTheoKhoaAsync(chuyenKhoaId);

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGioTrong(int bacSiId, int phongKhamId, string ngay)
        {
            try
            {
                var date = DateOnly.Parse(ngay);

                var gioTrong = await _buoiKhamService
                    .LayCacGioKhamTrongAsync(bacSiId, phongKhamId, date);

                return Json(new { success = true, gioTrong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message });
            }
        }

        // ==================== HỒ SƠ ====================
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var result = _leTanService.GetHoSo(id.Value);

            if (result == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin";
                return RedirectToAction("LeTanDashboard");
            }

            return View(result);
        }

        [HttpGet]
        public IActionResult HoSo()
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var result = _leTanService.GetHoSo(id.Value);

            if (result == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("LeTanDashboard");
            }

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoLeTanRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(HoSo));
            }

            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var (success, message) =
                _leTanService.CapNhatHoSo(id.Value, request);

            TempData[success ? "Success" : "Error"] = message;

            return RedirectToAction(nameof(ThongTinCaNhan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(HoSo));
            }

            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            if (request.MatKhauMoi != request.XacNhanMatKhauMoi)
            {
                TempData["Error"] = "Mật khẩu không khớp";
                return RedirectToAction(nameof(HoSo));
            }

            var (success, message) =
                await _leTanService.DoiMatKhau(id.Value, request);

            TempData[success ? "Success" : "Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }
    }
}