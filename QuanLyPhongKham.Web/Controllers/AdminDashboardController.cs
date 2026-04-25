using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Implementations;
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

        public async Task<IActionResult> TongQuan()
        {
            var listBacSi = _bacSiService.GetAll();
            ViewBag.TongBacSi = listBacSi != null ? listBacSi.Count() : 0;

            var listBenhNhan = await _benhNhanService.GetAllAsync();
            ViewBag.TongBenhNhan = listBenhNhan != null ? listBenhNhan.Count() : 0;

            var listLichKham = _buoiKhamService.GetAllLichKham();
            var homNay = DateOnly.FromDateTime(DateTime.Now);

            ViewBag.LichHomNay = listLichKham != null
                ? listLichKham.Count(l => l.Ngay == homNay)
                : 0;

            return View();
        }

        // --- QUẢN LÝ LỊCH TRỰC ---

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

                // 1. Lấy thông tin để hiển thị thông báo cho "oai"
                var bacSi = _bacSiService.GetById(BacSiId); // Giả sử hàm này có include Khoa
                var phong = (await _phongKhamService.GetAllAsync()).FirstOrDefault(p => p.Id == PhongKhamId);

                // 2. Gọi Service xử lý (Ông nên sửa Service để nó trả về danh sách ngày THÀNH CÔNG)
                var ketQua = await _lichTrucService.PhanCongNhieuNgayAsync(BacSiId, PhongKhamId, listDateObj);

                if (ketQua)
                {
                    // Gom danh sách ngày để hiển thị: "28/04, 29/04, 30/04"
                    var ngayHienThi = string.Join(", ", listDateObj.Select(d => d.ToString("dd/MM")));

                    TempData["ThongBao"] = $"✅ BS. {bacSi.HoTen} ({bacSi.ChuyenKhoa.TenKhoa}) - Phòng {phong.SoPhong} làm việc ngày: {ngayHienThi}";
                }
                else
                {
                    TempData["ThongBao"] = "⚠️ Không thể phân công (Lịch bị trùng bác sĩ hoặc trùng phòng)!";
                }
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi: " + ex.Message;
            }
            return RedirectToAction("LichTruc");
        }

        [HttpPost]
        public async Task<IActionResult> XoaLichTruc(int id)
        {
            try
            {
                var result = await _lichTrucService.XoaLichTrucAsync(id);
                if (result) TempData["ThongBao"] = "✅ Đã xóa lịch trực thành công!";
                else TempData["ThongBao"] = "❌ Lỗi: Không tìm thấy lịch trực này!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("LichTruc");
        }

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

        // --- QUẢN LÝ LỊCH KHÁM (BỆNH NHÂN ĐẶT) ---

        [HttpGet]
        public IActionResult LichKham()
        {
            var tatCaLich = _buoiKhamService.GetAllLichKham();
            return View(tatCaLich);
        }

        [HttpPost]
        public IActionResult DoiTrangThai(int id, int trangThaiMoi)
        {
            _buoiKhamService.CapNhatTrangThai(id, (TrangThaiBuoiKham)trangThaiMoi);
            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("LichKham");
        }

        [HttpPost]
        public IActionResult XoaLich(int id)
        {
            bool success = _buoiKhamService.XoaBuoiKham(id);
            if (success) TempData["Success"] = "Đã xóa lịch khám thành công!";
            else TempData["Error"] = "Lỗi: Không tìm thấy lịch khám này!";

            return RedirectToAction("LichKham");
        }

        // --- QUẢN LÝ BÁC SĨ ---

        [HttpGet]
        public IActionResult BacSi()
        {
            var tatCaBacSi = _bacSiService.GetAll();
            return View(tatCaBacSi);
        }
    }
}