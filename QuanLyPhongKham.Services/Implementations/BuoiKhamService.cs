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
        // GỌI HỘI REPOSITORY VÀO ĐÂY, ĐÁ BAY APP_DB_CONTEXT
        private readonly IBuoiKhamRepository _buoiKhamRepo;
        private readonly IBacSiRepository _bacSiRepo;
        private readonly IPhongKhamRepository _phongKhamRepo;
        private readonly IChuyenKhoaRepository _chuyenKhoaRepo;
        private readonly IBenhNhanRepository _benhNhanRepo;
        private readonly ILichTrucRepository _lichTrucRepo;

        public BuoiKhamService(
            IBuoiKhamRepository buoiKhamRepo,
            IBacSiRepository bacSiRepo,
            IPhongKhamRepository phongKhamRepo,
            IChuyenKhoaRepository chuyenKhoaRepo,
            IBenhNhanRepository benhNhanRepo,
            ILichTrucRepository lichTrucRepo)
        {
            _buoiKhamRepo = buoiKhamRepo;
            _bacSiRepo = bacSiRepo;
            _phongKhamRepo = phongKhamRepo;
            _chuyenKhoaRepo = chuyenKhoaRepo;
            _benhNhanRepo = benhNhanRepo;
            _lichTrucRepo = lichTrucRepo;
        }

        public async Task<object> LayBacSiVaPhongTheoKhoaAsync(int chuyenKhoaId)
        {
            var bacSisList = await _bacSiRepo.GetByChuyenKhoaIdAsync(chuyenKhoaId);
            var bacSis = bacSisList.Select(b => new {
                id = b.Id,
                ten = "BS. " + b.HoTen
            }).ToList();

            var phongsList = await _phongKhamRepo.GetByChuyenKhoaAsync(chuyenKhoaId);
            var phongs = phongsList.Select(p => new {
                id = p.Id,
                ten = "Phòng " + p.SoPhong + " (Tầng " + p.Tang + ") " + p.LoaiPhong
            }).ToList();

            return new { BacSis = bacSis, Phongs = phongs };
        }

        public async Task<List<ChuyenKhoa>> LayTatCaChuyenKhoaAsync()
        {
            return await _chuyenKhoaRepo.GetAllAsync();
        }

        public async Task<List<BenhNhan>> LayTatCaBenhNhanAsync()
        {
            var data = await _benhNhanRepo.GetAllAsync();
            return data.ToList(); 
        }

        public async Task<bool> DatLichKhamAsync(DatLichRequest request, int currentUserId, string role)
        {
            var homNay = DateOnly.FromDateTime(DateTime.Now);
            if (request.Ngay < homNay)
            {
                throw new Exception("Lỗi: Không thể đặt lịch cho ngày trong quá khứ!");
            }

            var cacCaDaDat = await _buoiKhamRepo.GetCacCaDaDatAsync(request.Ngay, request.Gio);

            bool trungBacSi = cacCaDaDat.Any(b => b.BacSiId == request.BacSiId);
            if (trungBacSi)
            {
                throw new Exception("Lỗi: Bác sĩ này đã có lịch trực hoặc lịch khám vào khung giờ này.");
            }

            bool trungPhong = cacCaDaDat.Any(b => b.PhongKhamId == request.PhongKhamId);
            if (trungPhong)
            {
                throw new Exception("Lỗi: Phòng khám này đã được sử dụng vào khung giờ này.");
            }

            var buoiKham = new BuoiKham
            {
                Ngay = request.Ngay,
                Gio = request.Gio,
                BacSiId = request.BacSiId,
                PhongKhamId = request.PhongKhamId,
                TrangThai = TrangThaiBuoiKham.ChuaXacNhan
            };

            if (role == "BenhNhan")
            {
                buoiKham.BenhNhanId = currentUserId;
            }
            else if (role == "Admin" || role == "LeTan")
            {
                if (!request.BenhNhanId.HasValue || request.BenhNhanId.Value <= 0)
                    throw new Exception("Vui lòng chọn bệnh nhân để đặt lịch hộ.");

                buoiKham.BenhNhanId = request.BenhNhanId.Value;
                buoiKham.TrangThai = TrangThaiBuoiKham.XacNhan;
            }
            else
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng này.");
            }

            return await _buoiKhamRepo.AddAsync(buoiKham);
        }

        public async Task<bool> CapNhatTrangThaiAsync(int buoiKhamId, TrangThaiBuoiKham trangThaiMoi, int currentUserId, string role)
        {
            var buoiKham = await _buoiKhamRepo.GetByIdAsync(buoiKhamId);
            if (buoiKham == null)
                throw new Exception("Không tìm thấy buổi khám.");

            if (role == "BacSi")
            {
                if (buoiKham.BacSiId != currentUserId)
                    throw new UnauthorizedAccessException("Bạn không thể sửa lịch của bác sĩ khác.");
            }
            else if (role == "BenhNhan")
            {
                if (buoiKham.BenhNhanId != currentUserId || trangThaiMoi != TrangThaiBuoiKham.Huy)
                    throw new UnauthorizedAccessException("Bệnh nhân chỉ được phép tự hủy lịch của mình.");
            }
            else if (role != "Admin" && role != "LeTan")
            {
                throw new UnauthorizedAccessException("Tài khoản không hợp lệ.");
            }

            buoiKham.TrangThai = trangThaiMoi;
            return await _buoiKhamRepo.UpdateAsync(buoiKham);
        }

        public List<BuoiKham> GetByBacSiId(int bacSiId)
        {
            return _buoiKhamRepo.GetByBacSiId(bacSiId);
        }

        public List<BuoiKham> GetByBenhNhanId(int benhNhanId)
        {
            return _buoiKhamRepo.GetByBenhNhanId(benhNhanId);
        }

        public List<BuoiKham> GetAllLichKham()
        {
            return _buoiKhamRepo.GetAll();
        }

        public BuoiKham GetById(int id)
        {
            var lich = _buoiKhamRepo.GetById(id);
            if (lich == null)
                throw new Exception("Không tìm thấy lịch");
            return lich;
        }

        public bool XoaBuoiKham(int id)
        {
            return _buoiKhamRepo.Delete(id);
        }

        public bool CapNhatTrangThai(int id, TrangThaiBuoiKham trangThaiMoi)
        {
            var lich = _buoiKhamRepo.GetById(id);
            if (lich == null)
                return false;

            lich.TrangThai = trangThaiMoi;
            return _buoiKhamRepo.Update(lich);
        }
        // =================================================================
        // ĐÃ CẬP NHẬT: XỬ LÝ SINH GIỜ BẰNG KIỂU DỮ LIỆU TIMEONLY
        // =================================================================
        public async Task<List<string>> LayCacGioKhamTrongAsync(int bacSiId, int phongKhamId, DateOnly ngayKham)
        {
            int phutMoiCa = 60; // Mặc định 60 phút

            var bacSi = _bacSiRepo.GetById(bacSiId);
            if (bacSi != null)
            {
                // Dùng switch case theo Id cho chuyên nghiệp và chính xác
                phutMoiCa = bacSi.ChuyenKhoaId switch
                {
                    2 or 6 => 30, // Khoa Răng, Răng Hàm Mặt
                    5 or 7 => 30, // Nhi, Tai Mũi Họng
                    8 => 20, // Da Liễu
                    _ => 60  // Các khoa còn lại (Nội, Ngoại...)
                };
            }

            var tatCaCaKham = new List<string>();

            // 2. Dùng TimeOnly để sinh danh sách Ca Sáng (07:00 -> 11:00)
            var batDauSang = new TimeOnly(7, 0);
            var ketThucSang = new TimeOnly(11, 0);
            var caHienTai = batDauSang;

            while (caHienTai.AddMinutes(phutMoiCa) <= ketThucSang)
            {
                var caTiepTheo = caHienTai.AddMinutes(phutMoiCa);
                // Format cứng HH:mm để string trả về đúng chuẩn hiển thị
                tatCaCaKham.Add($"{caHienTai:HH\\:mm} - {caTiepTheo:HH\\:mm}");
                caHienTai = caTiepTheo;
            }

            // 3. Dùng TimeOnly để sinh danh sách Ca Chiều (13:00 -> 17:00)
            var batDauChieu = new TimeOnly(13, 0);
            var ketThucChieu = new TimeOnly(17, 0);
            caHienTai = batDauChieu;

            while (caHienTai.AddMinutes(phutMoiCa) <= ketThucChieu)
            {
                var caTiepTheo = caHienTai.AddMinutes(phutMoiCa);
                tatCaCaKham.Add($"{caHienTai:HH\\:mm} - {caTiepTheo:HH\\:mm}");
                caHienTai = caTiepTheo;
            }

            // 4. Lấy các ca đã Kín (Repo trả về List<TimeOnly>)
            var cacGioDaDat = await _buoiKhamRepo.GetCacGioDaDatAsync(bacSiId, phongKhamId, ngayKham);

            // 5. Cộng thêm phút để tạo chuỗi so khớp
            var cacCaDaKin = cacGioDaDat.Select(g =>
            {
                var end = g.AddMinutes(phutMoiCa);
                return $"{g:HH\\:mm} - {end:HH\\:mm}";
            }).ToList();

            // 6. Trả về các ca còn Trống
            return tatCaCaKham.Except(cacCaDaKin).ToList();
        }
    }
}