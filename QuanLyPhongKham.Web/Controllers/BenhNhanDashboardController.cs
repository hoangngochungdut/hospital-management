using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;        
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Implementations;// new
using QuanLyPhongKham.Services.Interfaces;  // new

namespace QuanLyPhongKham.Web.Controllers
{
    public class BenhNhanDashboardController : Controller
    {
        // 1. CHỈ KHAI BÁO SERVICE, TUYỆT ĐỐI KHÔNG CÓ AppDbContext
        private readonly IBacSiService _bacSiService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly IBuoiKhamService _buoiKhamService;

        // 2. TIÊM SERVICE VÀO CONSTRUCTOR
        public BenhNhanDashboardController(
            IBacSiService bacSiService,
            IBenhNhanService benhNhanService,
            IBuoiKhamService buoiKhamService)
        {
            _bacSiService = bacSiService;
            _benhNhanService = benhNhanService;
            _buoiKhamService = buoiKhamService;
        }
        public IActionResult DanhGiaBS()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LichKham()
        {
            var dsKhoa = await _buoiKhamService.LayTatCaChuyenKhoaAsync();

            ViewBag.DsChuyenKhoa = dsKhoa;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DatLich(DateOnly Ngay, string Gio, int BacSiId, int PhongKhamId)
        {
            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            TimeOnly gioKhamChuan;
            if (!TimeOnly.TryParse(Gio, out gioKhamChuan))
            {
                TempData["ThongBao"] = "❌ Hệ thống không đọc được định dạng giờ (" + Gio + ")!";
                return RedirectToAction("LichKham");
            }

            // Gói dữ liệu vào DTO
            var request = new DatLichRequest
            {
                Ngay = Ngay,
                Gio = gioKhamChuan, // 3. GÁN BIẾN GIỜ ĐÃ ÉP KIỂU VÀO ĐÂY 👇
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId
            };

            try
            {
                var result = await _buoiKhamService.DatLichKhamAsync(request, currentBenhNhanId.Value, "BenhNhan");

                if (result)
                {
                    TempData["ThongBao"] = "✅ Đặt lịch thành công! Vui lòng chờ phòng khám xác nhận.";
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi trùng lịch, quá khứ từ Service ném ra
                TempData["ThongBao"] = ex.Message;
            }

            return RedirectToAction("LichKham");
        }
        // ==========================================================
        // CÁC HÀM XỬ LÝ AJAX (LẤY DỮ LIỆU ĐỘNG KHÔNG TẢI LẠI TRANG)
        // ==========================================================

        [HttpGet]
        public async Task<IActionResult> GetBacSiVaPhong(int chuyenKhoaId)
        {
            try
            {
                // Gọi Service lấy data (Hàm này ông đã viết ở BuoiKhamService rồi)
                var data = await _buoiKhamService.LayBacSiVaPhongTheoKhoaAsync(chuyenKhoaId);

                // Ném cục data đó ra dưới dạng văn bản JSON
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGioTrong(int bacSiId, int phongKhamId, string ngay)
        {
            try
            {
                var dateObj = DateOnly.Parse(ngay);
                var gioTrong = await _buoiKhamService.LayCacGioKhamTrongAsync(bacSiId, phongKhamId, dateObj);

                return Json(new { success = true, gioTrong = gioTrong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult XemLichKham()
        {
            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");
            if (currentBenhNhanId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lichCuaToi = _buoiKhamService.GetByBenhNhanId(currentBenhNhanId.Value);

            return View(lichCuaToi);
        }

        // GET: Thông tin cá nhân
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem thông tin";
                return RedirectToAction("Login", "Account");
            }

            var hoSo = _benhNhanService.GetHoSo(currentBenhNhanId.Value);

            if (hoSo == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("BenhNhanDashboard");
            }

            return View(hoSo);
        }


        // GET: Xem hồ sơ
        [HttpGet]
        public IActionResult HoSo()
        {
            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem hồ sơ";
                return RedirectToAction("Login", "Account");
            }

            var hoSo = _benhNhanService.GetHoSo(currentBenhNhanId.Value);

            if (hoSo == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("BenhNhanDashboard");
            }

            return View(hoSo);
        }


        // POST: Cập nhật hồ sơ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoBenhNhanRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại";
                return RedirectToAction(nameof(HoSo));
            }

            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để cập nhật hồ sơ";
                return RedirectToAction("Login", "Account");
            }

            var (success, message) = _benhNhanService.CapNhatHoSo(currentBenhNhanId.Value, request);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(ThongTinCaNhan));
        }


        // POST: Đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin";
                return RedirectToAction(nameof(HoSo));
            }

            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập";
                return RedirectToAction("Login", "Account");
            }

            var (success, message) = await _benhNhanService.DoiMatKhau(currentBenhNhanId.Value, request);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }
    }
}