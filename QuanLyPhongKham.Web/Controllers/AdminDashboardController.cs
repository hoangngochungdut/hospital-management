using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        #region TỔNG QUAN
        public async Task<IActionResult> TongQuan()
        {
            // Sửa lại cách đếm để tránh lỗi Null Reference
            var listBacSi = _bacSiService.GetAll();
            ViewBag.TongBacSi = listBacSi?.Count() ?? 0;

            var listBenhNhan = await _benhNhanService.GetAllAsync();
            ViewBag.TongBenhNhan = listBenhNhan?.Count() ?? 0;

            // Admin dùng hàm LayToanBoLichKhamAdminAsync để lấy đủ thông tin
            var listLichKham = await _buoiKhamService.LayToanBoLichKhamAdminAsync();
            var homNay = DateOnly.FromDateTime(DateTime.Now);

            ViewBag.LichHomNay = listLichKham?.Count(l => l.Ngay == homNay) ?? 0;

            return View();
        }
        #endregion

        #region QUẢN LÝ LỊCH TRỰC
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
                if (string.IsNullOrEmpty(DanhSachNgay)) throw new Exception("Vui lòng chọn ngày!");

                var mangNgayStr = DanhSachNgay.Split(',');
                var listDateObj = mangNgayStr.Select(str => DateOnly.Parse(str.Trim())).ToList();

                var bacSi = _bacSiService.GetById(BacSiId);
                var phongs = await _phongKhamService.GetAllAsync();
                var phong = phongs.FirstOrDefault(p => p.Id == PhongKhamId);

                var ketQua = await _lichTrucService.PhanCongNhieuNgayAsync(BacSiId, PhongKhamId, listDateObj);

                if (ketQua)
                {
                    var ngayHienThi = string.Join(", ", listDateObj.Select(d => d.ToString("dd/MM")));
                    TempData["ThongBao"] = $"✅ Đã phân công BS. {bacSi?.HoTen} trực tại Phòng {phong?.SoPhong} các ngày: {ngayHienThi}";
                }
                else
                {
                    TempData["ThongBao"] = "⚠️ Trùng lịch! Bác sĩ hoặc Phòng này đã có lịch trực trong danh sách ngày đã chọn.";
                }
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi: " + ex.Message;
            }
            return RedirectToAction("LichTruc");
        }
        #endregion

        #region QUẢN LÝ LỊCH KHÁM (BỆNH NHÂN ĐẶT)

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> QuanLyLichKham(int? chuyenKhoaId, string tenBacSi)
        {
            // Gọi Service và truyền tham số lọc xuống dưới
            var ketQuaLoc = await _buoiKhamService.LayToanBoLichKhamAdminAsync(chuyenKhoaId, tenBacSi);

            // Chuẩn bị dữ liệu cho các Dropdown ở View
            ViewBag.DsChuyenKhoa = await _buoiKhamService.LayTatCaChuyenKhoaAsync();
            ViewBag.SelectedKhoa = chuyenKhoaId;
            ViewBag.SelectedTen = tenBacSi;

            return View(ketQuaLoc);
        }

        [HttpPost]
        public IActionResult DoiTrangThai(int id, int trangThaiMoi)
        {
            // Sử dụng hàm CapNhatTrangThai (đồng bộ) đã viết ở BuoiKhamService
            _buoiKhamService.CapNhatTrangThai(id, (TrangThaiBuoiKham)trangThaiMoi, "Admin cập nhật");
            TempData["ThongBao"] = "✅ Cập nhật trạng thái thành công!";
            return RedirectToAction("QuanLyLichKham");
        }

        // Admin có thể xóa lịch nếu cần thiết (dùng cho trường hợp dọn rác hệ thống)
        [HttpPost]
        public IActionResult XoaLich(int id)
        {
            // Sử dụng hàm GetById sau đó xóa nếu cần, hoặc dùng hàm xóa trực tiếp nếu Repo hỗ trợ
            // Ở đây tạm dùng logic xóa đã có
            bool success = _buoiKhamService.CapNhatTrangThai(id, TrangThaiBuoiKham.Huy, "Admin xóa ca khám");
            TempData["ThongBao"] = success ? "✅ Đã hủy/xóa lịch khám!" : "❌ Không tìm thấy lịch!";
            return RedirectToAction("QuanLyLichKham");
        }
        // --- ACTION LẤY CHI TIẾT CA KHÁM CHO MODAL ---
        [HttpGet]
        public async Task<IActionResult> GetLichKhamDetail(int id)
        {
            // 1. Lấy ca khám theo ID (Nên dùng hàm có Include để lấy đủ thông tin liên quan)
            var lich = await _buoiKhamService.LayToanBoLichKhamAdminAsync();
            var detail = lich.FirstOrDefault(x => x.Id == id);

            if (detail == null) return NotFound();

            // 2. Map dữ liệu sang kiểu JSON đúng với các tên biến trong Javascript của ông
            return Json(new
            {
                benhNhanTen = detail.BenhNhan?.HoTen,
                sdt = detail.BenhNhan?.Sdt,
                diaChi = detail.BenhNhan?.DiaChi ?? "Chưa cập nhật",
                bacSiTen = detail.BacSi?.HoTen,
                khoa = detail.BacSi?.ChuyenKhoa?.TenKhoa,
                soPhong = detail.PhongKham?.SoPhong,
                // Ghi chú này có thể là lý do dời lịch hoặc kết quả khám
                ghiChu = detail.GhiChuKetQua ?? detail.ThongBaoChoBenhNhan ?? "Không có ghi chú đặc biệt."
            });
        }
        #endregion

        #region AJAX DATA
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
        #endregion
    }
}