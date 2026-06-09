using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers.AdminDashboardController
{
    public class AdminDashboardController : Controller
    {
        private readonly IBacSiService _bacSiService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly IChuyenKhoaService _chuyenKhoaService;
        private readonly IBuoiKhamService _buoiKhamService;
        private readonly ILichTrucService _lichTrucService;
        private readonly IPhongKhamService _phongKhamService;   
        private readonly ILeTanService _leTanService;

        public AdminDashboardController(
            IBuoiKhamService buoiKhamService,
            IBacSiService bacSiService,
            IBenhNhanService benhNhanService,
            IPhongKhamService phongKhamService,
            ILichTrucService lichTrucService,
            IChuyenKhoaService chuyenKhoaService,
            ILeTanService leTanService)
        {
            _buoiKhamService = buoiKhamService;
            _chuyenKhoaService = chuyenKhoaService;
            _bacSiService = bacSiService;
            _benhNhanService = benhNhanService;
            _phongKhamService = phongKhamService;
            _lichTrucService = lichTrucService;
            _leTanService = leTanService;
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
            ViewBag.DsChuyenKhoa = _chuyenKhoaService.GetAll();
            var danhSachLich = await _lichTrucService.LayTatCaLichTrucAsync();
            return View(danhSachLich);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken] 
        public async Task<IActionResult> ImportLichTrucExcel([FromBody] List<LichTrucImportDto> data)
        {
            if (data == null || !data.Any())
            {
                return Json(new { success = false, message = "Không có dữ liệu hợp lệ!" });
            }

            int countSuccess = 0;
            int countTrach = 0; 

            foreach (var item in data)
            {
                if (DateOnly.TryParseExact(item.Ngay, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateOnly ngayTruc))
                {
                    try
                    {
                        bool ketQua = await _lichTrucService.PhanCongBacSiAsync(item.BacSiId, item.PhongKhamId, ngayTruc);
                        if (ketQua)
                        {
                            countSuccess++;
                        }
                    }
                    catch (Exception)
                    {
                        countTrach++;
                    }
                }
            }

            if (countSuccess > 0)
            {
                return Json(new { success = true, message = $"Nhập Excel hoàn tất! Thêm mới {countSuccess} ca. Bỏ qua {countTrach} ca do trùng lịch." });
            }
            else
            {
                return Json(new { success = false, message = $"Không có ca trực nào được thêm (Trùng lịch {countTrach} ca)." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> LuuPhanCong(int BacSiId, int PhongKhamId, string DanhSachNgay)
        {
            try
            {
                if (string.IsNullOrEmpty(DanhSachNgay))
                    throw new Exception("Vui lòng chọn ngày trên lịch!");
                var listDateObj = DanhSachNgay
                    .Split(',')
                    .Select(str => DateOnly.ParseExact(str.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
                    .ToList();

                var bacSi = _bacSiService.GetById(BacSiId);
                var phongs = await _phongKhamService.GetAllAsync();
                var phong = phongs.FirstOrDefault(p => p.Id == PhongKhamId);

                List<string> successDates = new List<string>();
                List<string> conflictDates = new List<string>();

                foreach (var date in listDateObj)
                {
                    try
                    {
                        bool isSuccess = await _lichTrucService.PhanCongBacSiAsync(BacSiId, PhongKhamId, date);
                        if (isSuccess)
                        {
                            successDates.Add(date.ToString("dd/MM"));
                        }
                        else
                        {
                            conflictDates.Add(date.ToString("dd/MM"));
                        }
                    }
                    catch (Exception)
                    {
                        conflictDates.Add(date.ToString("dd/MM"));
                    }
                }

                if (successDates.Any())
                {
                    string message = $"✅ Đã phân công BS. {bacSi?.HoTen} tại Phòng {phong?.SoPhong} các ngày: {string.Join(", ", successDates)}.";
                    if (conflictDates.Any())
                    {
                        message += $" <br/>⚠️ Bỏ qua các ngày bị trùng lịch: {string.Join(", ", conflictDates)}.";
                    }
                    TempData["ThongBao"] = message;
                }
                else
                {
                    TempData["ThongBao"] = $"❌ Phân công thất bại! Tất cả các ngày đã chọn đều bị trùng lịch: {string.Join(", ", conflictDates)}.";
                }
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi: " + ex.Message;
            }

            return RedirectToAction("LichTruc");
        }
        [HttpPost]
        public async Task<IActionResult> SaoChepLichTuanTruocAjax(string ngayThuHaiHienTai)
        {
            try
            {
                if (!DateOnly.TryParse(ngayThuHaiHienTai, out DateOnly thuHaiHienTai))
                    return Json(new { success = false, message = "Ngày không hợp lệ!" });

                DateOnly thuHaiTuanTruoc = thuHaiHienTai.AddDays(-7);

                var tatCaLich = await _lichTrucService.LayTatCaLichTrucAsync();

                var lichTuanTruoc = tatCaLich.Where(x => x.Ngay >= thuHaiTuanTruoc && x.Ngay < thuHaiHienTai).ToList();

                if (!lichTuanTruoc.Any())
                {
                    return Json(new { success = false, message = "Không tìm thấy dữ liệu lịch trực của tuần trước để thực hiện sao chép!" });
                }

                int countSuccess = 0;
                foreach (var item in lichTuanTruoc)
                {
                    DateOnly ngayTuanNay = item.Ngay.AddDays(7); 

                    bool checkLuu = await _lichTrucService.PhanCongBacSiAsync(item.BacSiId, item.PhongKhamId, ngayTuanNay);
                    if (checkLuu) countSuccess++;
                }

                return Json(new { success = true, message = $"🚀 Hoàn tất! Đã sao chép nhanh {countSuccess} ca trực từ tuần trước sang tuần này thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaLichHangLoat([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Json(new { success = false, message = "Không có lịch nào được chọn!" });

            var result = await _lichTrucService.XoaLichTheoDanhSachIdAsync(ids);
            if (result)
                return Json(new { success = true, message = $"Đã xóa thành công {ids.Count} lịch trực!" });

            return Json(new { success = false, message = "Lỗi khi xóa lịch trực!" });
        }

        // ==================== QUẢN LÝ LỊCH KHÁM ====================
        [HttpGet]
        public async Task<IActionResult> QuanLyLichKham(int? chuyenKhoaId, string? tenBacSi)
        {
            var ketQuaLoc = await _buoiKhamService.LayToanBoLichKhamAdminAsync(chuyenKhoaId, tenBacSi);
            return View(ketQuaLoc);
        }

        [HttpPost]
        public IActionResult DoiTrangThai(int id, int trangThaiMoi)
        {
            try
            {
                _buoiKhamService.XulyCaKham(
                    id,
                    (TrangThaiBuoiKham)trangThaiMoi,
                    "Admin cập nhật"
                );
                TempData["ThongBao"] = "✅ Cập nhật trạng thái thành công!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi: " + ex.Message;
            }
            return RedirectToAction("QuanLyLichKham");
        }
        // 🔥 THÊM MỚI: ADMIN DỜI LỊCH
        [HttpPost]
        public IActionResult DoiLichKham(int id, DateTime ngayMoi, string gioMoi, string lyDoDoi)
        {
            try
            {
                DateOnly dateParsed = DateOnly.FromDateTime(ngayMoi);
                TimeOnly timeParsed = TimeOnly.Parse(gioMoi);

                _buoiKhamService.DoiLichKham(id, dateParsed, timeParsed, lyDoDoi);
                TempData["ThongBao"] = "✅ Dời lịch thành công! Cập nhật đã được ghi nhận.";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Không thể dời lịch: " + ex.Message;
            }

            return RedirectToAction("QuanLyLichKham");
        }

        //  ADMIN: Xóa
        [HttpPost]
        public IActionResult XoaVinhVien(int id)
        {
            try
            {
                _buoiKhamService.XoaLichKham(id); 
                TempData["ThongBao"] = "✅ Đã xóa vĩnh viễn lịch khám khỏi hệ thống!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi xóa lịch: " + ex.Message;
            }
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
        [HttpGet]
        public async Task<IActionResult> GetTuDienBacSiPhong()
        {
            try
            {
                var listBacSi = _bacSiService.GetAll();
                var listPhong = await _phongKhamService.GetAllAsync();
                var bacSis = listBacSi.Select(b => new {
                    id = b.Id,
                    hoTen = b.HoTen,
                    khoa = b.ChuyenKhoa?.TenKhoa ?? "N/A"
                });

                var phongs = listPhong.Select(p => new {
                    id = p.Id,
                    soPhong = p.SoPhong.ToString() 
                });

                return Json(new { success = true, bacSis = bacSis, phongs = phongs });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ==================== AJAX KHÁC ====================
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

        public IActionResult BacSi()
        {
            var tatCaBacSi = _bacSiService.GetAll();
            return View(tatCaBacSi);
        }
        public IActionResult BenhNhan()
        {
            var tatCaBenhNhan = _benhNhanService.GetAll();
            return View(tatCaBenhNhan);
        }

        public IActionResult ChuyenKhoa()
        {
            var tatCaChuyenKhoa = _chuyenKhoaService.GetAll();
            return View(tatCaChuyenKhoa);
        }
        public IActionResult LeTan()
        {
            var tatCaLeTan = _leTanService.GetAll();
            return View(tatCaLeTan);
        }
    }



}