using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BenhNhanDashboardController : Controller
    {
        private readonly IBacSiService _bacSiService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly IBuoiKhamService _buoiKhamService;

        public BenhNhanDashboardController(
            IBacSiService bacSiService,
            IBenhNhanService benhNhanService,
            IBuoiKhamService buoiKhamService)
        {
            _bacSiService = bacSiService;
            _benhNhanService = benhNhanService;
            _buoiKhamService = buoiKhamService;
        }

        // ==================== ĐẶT LỊCH ====================
        [HttpGet]
        public async Task<IActionResult> LichKham()
        {
            ViewBag.DsChuyenKhoa = await _buoiKhamService.LayTatCaChuyenKhoaAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DatLich(
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
                TempData["ThongBao"] = $"❌ Không đọc được giờ ({Gio})!";
                return RedirectToAction("LichKham");
            }

            var request = new DatLichRequest
            {
                Ngay = Ngay,
                Gio = gioParsed,
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId
            };

            try
            {
                var result = await _buoiKhamService.DatLichKhamAsync(request, currentId.Value, "BenhNhan");

                if (result)
                    TempData["ThongBao"] = "✅ Đặt lịch thành công! Chờ xác nhận.";
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
                var data = await _buoiKhamService.LayBacSiVaPhongTheoKhoaAsync(chuyenKhoaId);
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
                var gioTrong = await _buoiKhamService.LayCacGioKhamTrongAsync(bacSiId, phongKhamId, date);
                return Json(new { success = true, gioTrong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message });
            }
        }

        // ==================== LỊCH CỦA TÔI ====================
        [HttpGet]
        public IActionResult XemLichKham()
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");

            if (currentId == null)
                return RedirectToAction("Login", "Account");

            var lich = _buoiKhamService.GetByBenhNhanId(currentId.Value);

            return View(lich);
        }

        [HttpPost]
        public IActionResult BenhNhanHuyLich(int id, string lyDo)
        {
            try
            {
                _buoiKhamService.XulyCaKham(id, TrangThaiBuoiKham.Huy, lyDo);
                TempData["ThongBao"] = "✅ Đã hủy lịch!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ " + ex.Message;
            }

            return RedirectToAction("XemLichKham");
        }

        [HttpPost]
        public IActionResult BenhNhanDoiLich(int id, DateTime ngayMoi, string gioMoi, string lyDo)
        {
            try
            {
                var date = DateOnly.FromDateTime(ngayMoi);
                var time = TimeOnly.Parse(gioMoi);

                _buoiKhamService.BenhNhanYeuCauDoiLich(id, date, time, lyDo);
                TempData["ThongBao"] = "✅ Đã gửi yêu cầu dời lịch!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ " + ex.Message;
            }

            return RedirectToAction("XemLichKham");
        }

        // ==================== ĐÁNH GIÁ ====================
        [HttpPost]
        public IActionResult NopDanhGia(int id, int soSao, string nhanXet)
        {
            try
            {
                _buoiKhamService.LuuDanhGiaCuaBenhNhan(id, soSao, nhanXet);
                TempData["ThongBao"] = "✅ Đánh giá thành công!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ " + ex.Message;
            }

            return RedirectToAction("XemLichKham");
        }

        // ==================== XEM BÁC SĨ ====================
        [HttpGet]
        public IActionResult XemBacSi()
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");

            if (currentId == null)
                return RedirectToAction("Login", "Account");

            var ds = _bacSiService.LayDanhSachBacSiVaDanhGia();

            return View(ds);
        }

        // ==================== HỒ SƠ ====================
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var hoSo = _benhNhanService.GetHoSo(id.Value);

            if (hoSo == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("BenhNhanDashboard");
            }

            return View(hoSo);
        }

        [HttpGet]
        public IActionResult HoSo()
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var hoSo = _benhNhanService.GetHoSo(id.Value);

            if (hoSo == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("BenhNhanDashboard");
            }

            return View(hoSo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoBenhNhanRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(HoSo));
            }

            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var (success, message) = _benhNhanService.CapNhatHoSo(id.Value, request);
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

            var (success, message) = await _benhNhanService.DoiMatKhau(id.Value, request);
            TempData[success ? "Success" : "Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }
    }
}