using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IBacSiService _bacSiService;
        private readonly IBuoiKhamService _buoiKhamService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly ILichTrucService _lichTrucService;
        private readonly IPhongKhamService _phongKhamService;

        public AdminDashboardController(
            IBuoiKhamService buoiKhamService,
            IBacSiService bacSiService,
            IBenhNhanService benhNhanService,
            IPhongKhamService phongKhamService,
            ILichTrucService lichTrucService)
        {
            _buoiKhamService = buoiKhamService;
            _bacSiService = bacSiService;
            _benhNhanService = benhNhanService;
            _phongKhamService = phongKhamService;
            _lichTrucService = lichTrucService;
        }

        // ==================== TỔNG QUAN ====================
        public async Task<IActionResult> TongQuan()
        {
            var listBacSi = _bacSiService.GetAll();
            ViewBag.TongBacSi = listBacSi?.Count() ?? 0;

            var listBenhNhan = await _benhNhanService.GetAllAsync();
            ViewBag.TongBenhNhan = listBenhNhan?.Count() ?? 0;

            var listLichKham = await _buoiKhamService.LayToanBoLichKhamAdminAsync();
            var homNay = DateOnly.FromDateTime(DateTime.Now);

            ViewBag.LichHomNay = listLichKham?.Count(l => l.Ngay == homNay) ?? 0;

            return View();
        }

        // ==================== LỊCH TRỰC ====================
        [HttpGet]
        public async Task<IActionResult> LichTruc()
        {
            ViewBag.DsChuyenKhoa = _bacSiService.GetDanhSachChuyenKhoa();
            var danhSachLich = await _lichTrucService.LayTatCaLichTrucAsync();
            return View(danhSachLich);
        }

        [HttpPost]
        public async Task<IActionResult> LuuPhanCong(int BacSiId, int PhongKhamId, string DanhSachNgay)
        {
            try
            {
                if (string.IsNullOrEmpty(DanhSachNgay))
                    throw new Exception("Vui lòng chọn ngày!");

                var listDateObj = DanhSachNgay
                    .Split(',')
                    .Select(str => DateOnly.Parse(str.Trim()))
                    .ToList();

                var bacSi = _bacSiService.GetById(BacSiId);
                var phongs = await _phongKhamService.GetAllAsync();
                var phong = phongs.FirstOrDefault(p => p.Id == PhongKhamId);

                var ketQua = await _lichTrucService
                    .PhanCongNhieuNgayAsync(BacSiId, PhongKhamId, listDateObj);

                if (ketQua)
                {
                    var ngayHienThi = string.Join(", ",
                        listDateObj.Select(d => d.ToString("dd/MM")));

                    TempData["ThongBao"] =
                        $"✅ Đã phân công BS. {bacSi?.HoTen} trực tại Phòng {phong?.SoPhong} các ngày: {ngayHienThi}";
                }
                else
                {
                    TempData["ThongBao"] =
                        "⚠️ Trùng lịch! Bác sĩ hoặc Phòng đã có lịch.";
                }
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi: " + ex.Message;
            }

            return RedirectToAction("LichTruc");
        }

        // ==================== LỊCH KHÁM ====================
        [HttpGet]
        public async Task<IActionResult> QuanLyLichKham(int? chuyenKhoaId, string? tenBacSi)
        {
            var ketQuaLoc = await _buoiKhamService
                .LayToanBoLichKhamAdminAsync(chuyenKhoaId, tenBacSi);

            ViewBag.DsChuyenKhoa = await _buoiKhamService.LayTatCaChuyenKhoaAsync();
            ViewBag.SelectedKhoa = chuyenKhoaId;
            ViewBag.SelectedTen = tenBacSi;

            return View(ketQuaLoc);
        }

        [HttpPost]
        public IActionResult DoiTrangThai(int id, int trangThaiMoi)
        {
            _buoiKhamService.CapNhatTrangThai(
                id,
                (TrangThaiBuoiKham)trangThaiMoi,
                "Admin cập nhật"
            );

            TempData["ThongBao"] = "✅ Cập nhật trạng thái thành công!";
            return RedirectToAction("QuanLyLichKham");
        }

        [HttpPost]
        public IActionResult XoaLich(int id)
        {
            bool success = _buoiKhamService.CapNhatTrangThai(
                id,
                TrangThaiBuoiKham.Huy,
                "Admin xóa ca khám"
            );

            TempData["ThongBao"] = success
                ? "✅ Đã hủy/xóa lịch khám!"
                : "❌ Không tìm thấy lịch!";

            return RedirectToAction("QuanLyLichKham");
        }

        [HttpGet]
        public async Task<IActionResult> GetLichKhamDetail(int id)
        {
            var lich = await _buoiKhamService.LayToanBoLichKhamAdminAsync();
            var detail = lich.FirstOrDefault(x => x.Id == id);

            if (detail == null) return NotFound();

            return Json(new
            {
                benhNhanTen = detail.BenhNhan?.HoTen,
                sdt = detail.BenhNhan?.Sdt,
                diaChi = detail.BenhNhan?.DiaChi ?? "Chưa cập nhật",
                bacSiTen = detail.BacSi?.HoTen,
                khoa = detail.BacSi?.ChuyenKhoa?.TenKhoa,
                soPhong = detail.PhongKham?.SoPhong,
                ghiChu = detail.GhiChuKetQua
                           ?? detail.ThongBaoChoBenhNhan
                           ?? "Không có ghi chú đặc biệt."
            });
        }

        // ==================== AJAX ====================
        [HttpGet]
        public async Task<IActionResult> GetDataByKhoa(int chuyenKhoaId)
        {
            var bacSis = await _bacSiService.GetByChuyenKhoaIdAsync(chuyenKhoaId);
            var phongs = await _phongKhamService.GetByChuyenKhoaAsync(chuyenKhoaId);

            return Json(new
            {
                success = true,
                bacSis = bacSis.Select(b => new { b.Id, b.HoTen }),
                phongs = phongs.Select(p => new { p.Id, p.SoPhong, p.LoaiPhong })
            });
        }
    }
}