using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BacSiDashboardController : Controller
    {
        private readonly IBacSiService _bacSiService;
        private readonly IChuyenKhoaService _chuyenKhoaService;
        private readonly IBuoiKhamService _buoiKhamService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly ILichTrucService _lichTrucService;

        public BacSiDashboardController(
            IBacSiService bacSiService,
            IBuoiKhamService buoiKhamService,
            IBenhNhanService benhNhanService,
            IChuyenKhoaService chuyenKhoaService,
            ILichTrucService lichTrucService) 
        {
            _bacSiService = bacSiService;
            _chuyenKhoaService = chuyenKhoaService;
            _buoiKhamService = buoiKhamService;
            _benhNhanService = benhNhanService;
            _lichTrucService = lichTrucService;
        }
        // ==================== LỊCH KHÁM ====================
        [HttpGet]
        public IActionResult LichKham()
        {
            int? currentBacSiId = HttpContext.Session.GetInt32("UserId");

            if (currentBacSiId == null)
                return RedirectToAction("Login", "Account");

            var lichCuaToi = _buoiKhamService.GetByBacSiId(currentBacSiId.Value);

            return View(lichCuaToi);
        }

        [HttpPost]
        public IActionResult DoiTrangThai(int id, int trangThaiMoi, string? ghiChu, string? ketQuaKhamBenh)
        {
            try
            {
                bool isSuccess = _buoiKhamService.XulyCaKham(
                    id,
                    (TrangThaiBuoiKham)trangThaiMoi,
                    ghiChu,
                    ketQuaKhamBenh
                );

                if (isSuccess)
                {
                    TempData["ThongBao"] = "✅ Xử lý ca khám thành công!";
                }
                else
                {
                    TempData["ThongBao"] = "❌ Lỗi: Không tìm thấy ca khám!";
                }
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Cập nhật thất bại: " + ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        // Dời lịch khám
        [HttpPost]
        public IActionResult DoiLichKham(int id, DateTime ngayMoi, string gioMoi, string lyDoDoi)
        {
            try
            {
                DateOnly dateParsed = DateOnly.FromDateTime(ngayMoi);
                TimeOnly timeParsed = TimeOnly.Parse(gioMoi);

                _buoiKhamService.DoiLichKham(id, dateParsed, timeParsed, lyDoDoi);

                TempData["ThongBao"] =
                    "✅ Dời lịch thành công! Bệnh nhân đã nhận được thông báo.";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] =
                    "❌ Không thể dời lịch: " + ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        // ==================== HỒ SƠ ====================
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var result = _bacSiService.GetHoSo(userId.Value);

            if (result == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin";
                return RedirectToAction("BacSiDashboard");
            }

            return View(result);
        }

        [HttpGet]
        public IActionResult HoSo()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var result = _bacSiService.GetHoSo(userId.Value);

            // ✅ gọi qua service
            ViewBag.DanhSachChuyenKhoa = _chuyenKhoaService.GetAll();

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoBacSiRequest request)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ!";
                return RedirectToAction(nameof(HoSo));
            }

            var (success, message) =
                _bacSiService.CapNhatHoSo(userId.Value, request);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(ThongTinCaNhan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauRequest request)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (request.MatKhauMoi != request.XacNhanMatKhauMoi)
            {
                TempData["Error"] = "Mật khẩu mới và xác nhận không khớp";
                return RedirectToAction(nameof(HoSo));
            }

            var (success, message) =
                await _bacSiService.DoiMatKhau(userId.Value, request);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }

        public IActionResult TieuSuBenhNhan(int id)
        {
            try
            {
                var data = _benhNhanService.GetTieuSu(id);

                if (data == null)
                    return Content("DATA NULL");

                return PartialView(data);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString()); 
            }
        }
        public async Task<IActionResult> LichTruc()
        {
            // 1. Lấy NguoiDungId từ Session (Và đây CHÍNH LÀ BacSiId)
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");

            // Nếu chưa đăng nhập thì đá ra trang Login
            if (sessionUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Chốt luôn ID! Không cần query tìm kiếm lằng nhằng nữa
            int bacSiIdChuan = sessionUserId.Value;

            // 3. Đưa thẳng ID vào Service lấy lịch trực
            var danhSachLichTruc = await _lichTrucService.LayLichTrucTuHomNayAsync(bacSiIdChuan);

            return View(danhSachLichTruc);
        }
    }
}