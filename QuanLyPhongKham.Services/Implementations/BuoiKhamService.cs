using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Implementations
{
    public class BuoiKhamService : IBuoiKhamService
    {
        private readonly IBuoiKhamRepository _buoiKhamRepo;
        private readonly IBacSiRepository _bacSiRepo;
        private readonly IPhongKhamRepository _phongKhamRepo;
        private readonly IChuyenKhoaRepository _chuyenKhoaRepo;
        private readonly IBenhNhanRepository _benhNhanRepo;

        public BuoiKhamService(
            IBuoiKhamRepository buoiKhamRepo,
            IBacSiRepository bacSiRepo,
            IPhongKhamRepository phongKhamRepo,
            IChuyenKhoaRepository chuyenKhoaRepo,
            IBenhNhanRepository benhNhanRepo)
        {
            _buoiKhamRepo = buoiKhamRepo;
            _bacSiRepo = bacSiRepo;
            _phongKhamRepo = phongKhamRepo;
            _chuyenKhoaRepo = chuyenKhoaRepo;
            _benhNhanRepo = benhNhanRepo;
        }

        #region QUẢN LÝ DANH SÁCH (READ)

        // Dành cho Admin: Lấy tất cả lịch khám kèm theo Bác sĩ, Bệnh nhân, Phòng
        public async Task<List<BuoiKham>> LayToanBoLichKhamAdminAsync(int? chuyenKhoaId = null, string? tenBacSi = null)
        {
            // 1. Lấy data thô đã Include đầy đủ từ Repo
            var listLich = await _buoiKhamRepo.GetAllLichKhamFullAsync();

            // 2. Ép kiểu sang Queryable để lọc cho mượt (hoặc lọc trực tiếp trên List)
            var query = listLich.AsQueryable();

            // 3. Xử lý logic lọc tại Service
            if (chuyenKhoaId.HasValue && chuyenKhoaId > 0)
            {
                // Thay vì: l => l.BacSi?.ChuyenKhoaId == chuyenKhoaId
                query = query.Where(l => l.BacSi != null && l.BacSi.ChuyenKhoaId == chuyenKhoaId);
            }

            if (!string.IsNullOrEmpty(tenBacSi))
            {
                var searchName = tenBacSi.ToLower();
                query = query.Where(l => l.BacSi != null && l.BacSi.HoTen.ToLower().Contains(searchName));
            }

            return query.ToList();
        }

        public List<BuoiKham> GetByBacSiId(int bacSiId) => _buoiKhamRepo.GetByBacSiId(bacSiId);

        public List<BuoiKham> GetByBenhNhanId(int benhNhanId) => _buoiKhamRepo.GetByBenhNhanId(benhNhanId);

        public BuoiKham GetById(int id) => _buoiKhamRepo.GetById(id) ?? throw new Exception("Không tìm thấy lịch khám");

        public async Task<List<ChuyenKhoa>> LayTatCaChuyenKhoaAsync() => await _chuyenKhoaRepo.GetAllAsync();

        #endregion

        #region NGHIỆP VỤ ĐẶT LỊCH & KIỂM TRA TRÙNG

        public async Task<object> LayBacSiVaPhongTheoKhoaAsync(int chuyenKhoaId)
        {
            var bacSisList = await _bacSiRepo.GetByChuyenKhoaIdAsync(chuyenKhoaId);
            var phongsList = await _phongKhamRepo.GetByChuyenKhoaAsync(chuyenKhoaId);

            return new
            {
                BacSis = bacSisList.Select(b => new { id = b.Id, ten = "BS. " + b.HoTen }),
                Phongs = phongsList.Select(p => new { id = p.Id, ten = $"Phòng {p.SoPhong} (Tầng {p.Tang})" })
            };
        }

        public async Task<bool> DatLichKhamAsync(DatLichRequest request, int currentUserId, string role)
        {
            if (request.Ngay < DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Lỗi: Không thể đặt lịch cho quá khứ!");

            // Kiểm tra trùng lịch bác sĩ/phòng
            var cacCaDaDat = await _buoiKhamRepo.GetCacCaDaDatAsync(request.Ngay, request.Gio);
            if (cacCaDaDat.Any(b => b.BacSiId == request.BacSiId)) throw new Exception("Lỗi: Bác sĩ đã có lịch tại khung giờ này.");
            if (cacCaDaDat.Any(b => b.PhongKhamId == request.PhongKhamId)) throw new Exception("Lỗi: Phòng khám đã kín chỗ.");

            var buoiKham = new BuoiKham
            {
                Ngay = request.Ngay,
                Gio = request.Gio,
                BacSiId = request.BacSiId,
                PhongKhamId = request.PhongKhamId,
                TrangThai = (role == "BenhNhan") ? TrangThaiBuoiKham.ChuaXacNhan : TrangThaiBuoiKham.XacNhan,
                BenhNhanId = (role == "BenhNhan") ? currentUserId : (request.BenhNhanId ?? throw new Exception("Thiếu ID bệnh nhân"))
            };

            return await _buoiKhamRepo.AddAsync(buoiKham);
        }

        public async Task<List<string>> LayCacGioKhamTrongAsync(int bacSiId, int phongKhamId, DateOnly ngayKham)
        {
            // Logic tính số phút mỗi ca dựa trên khoa (như cũ của ông)
            int phutMoiCa = 60;
            var bacSi = _bacSiRepo.GetById(bacSiId);
            if (bacSi != null) phutMoiCa = bacSi.ChuyenKhoaId switch { 2 or 5 or 6 or 7 => 30, 8 => 20, _ => 60 };

            var tatCaCaKham = new List<string>();
            Action<TimeOnly, TimeOnly> sinhCa = (start, end) => {
                while (start.AddMinutes(phutMoiCa) <= end)
                {
                    var next = start.AddMinutes(phutMoiCa);
                    tatCaCaKham.Add($"{start:HH:mm} - {next:HH:mm}");
                    start = next;
                }
            };

            sinhCa(new TimeOnly(7, 0), new TimeOnly(11, 0));  // Sáng
            sinhCa(new TimeOnly(13, 0), new TimeOnly(17, 0)); // Chiều

            var cacGioDaDat = await _buoiKhamRepo.GetCacGioDaDatAsync(bacSiId, phongKhamId, ngayKham);
            var cacCaDaKin = cacGioDaDat.Select(g => $"{g:HH:mm} - {g.AddMinutes(phutMoiCa):HH:mm}");

            return tatCaCaKham.Except(cacCaDaKin).ToList();
        }

        #endregion

        #region CẬP NHẬT TRẠNG THÁI & DỜI LỊCH

        public bool CapNhatTrangThai(int id, TrangThaiBuoiKham trangThaiMoi, string ghiChu = null)
        {
            var lich = _buoiKhamRepo.GetById(id);
            if (lich == null) return false;

            lich.TrangThai = trangThaiMoi;

            if (trangThaiMoi == TrangThaiBuoiKham.HoanThanh)
            {
                lich.GhiChuKetQua = ghiChu;
                lich.ThongBaoChoBenhNhan = "✅ Lịch khám đã hoàn thành. Vui lòng xem kết quả.";
            }
            else if (trangThaiMoi == TrangThaiBuoiKham.Huy)
            {
                lich.ThongBaoChoBenhNhan = "❌ Lịch đã bị hủy. Lý do: " + (ghiChu ?? "Không có lý do cụ thể");
            }
            else if (trangThaiMoi == TrangThaiBuoiKham.XacNhan)
            {
                if (string.IsNullOrEmpty(lich.ThongBaoChoBenhNhan) || !lich.ThongBaoChoBenhNhan.Contains("dời lịch"))
                    lich.ThongBaoChoBenhNhan = "✅ Bác sĩ đã xác nhận lịch khám của bạn.";
            }

            return _buoiKhamRepo.Update(lich);
        }

        public bool DoiLichKham(int id, DateOnly ngayMoi, TimeOnly gioMoi, string lyDo)
        {
            var lich = _buoiKhamRepo.GetById(id);
            if (lich == null || lich.TrangThai == TrangThaiBuoiKham.HoanThanh || lich.TrangThai == TrangThaiBuoiKham.Huy)
                throw new Exception("Không thể thay đổi lịch đã kết thúc.");

            lich.Ngay = ngayMoi;
            lich.Gio = gioMoi;
            lich.TrangThai = TrangThaiBuoiKham.XacNhan;
            lich.ThongBaoChoBenhNhan = $"🔄 Bác sĩ dời lịch từ {lich.Ngay:dd/MM} sang {ngayMoi:dd/MM} lúc {gioMoi:HH:mm}. Lý do: {lyDo}";

            return _buoiKhamRepo.Update(lich);
        }

        public bool BenhNhanYeuCauDoiLich(int id, DateOnly ngayMoi, TimeOnly gioMoi, string lyDo)
        {
            var lich = _buoiKhamRepo.GetById(id);
            if (lich == null) return false;

            lich.Ngay = ngayMoi;
            lich.Gio = gioMoi;
            lich.TrangThai = TrangThaiBuoiKham.ChuaXacNhan;
            lich.ThongBaoChoBenhNhan = $"📌 Bạn yêu cầu dời lịch sang {ngayMoi:dd/MM} lúc {gioMoi:HH:mm}. Lý do: {lyDo}. Đang chờ xác nhận.";

            return _buoiKhamRepo.Update(lich);
        }

        public bool LuuDanhGiaCuaBenhNhan(int id, int soSao, string nhanXet)
        {
            var lich = _buoiKhamRepo.GetById(id);
            if (lich == null || lich.TrangThai != TrangThaiBuoiKham.HoanThanh) return false;

            lich.DiemDanhGia = soSao;
            lich.NhanXetCuaBenhNhan = nhanXet;
            return _buoiKhamRepo.Update(lich);
        }

        #endregion
    }
}