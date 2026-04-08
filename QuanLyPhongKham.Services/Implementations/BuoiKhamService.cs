using System;
using System.Threading.Tasks;
using QuanLyPhongKham.Models; // Chứa class BuoiKham
using QuanLyPhongKham.Models.DTOs; // Chứa DatLichRequest
using QuanLyPhongKham.Models.Enums; // Chứa Enum TrangThai
using QuanLyPhongKham.Repositories.Interfaces; // Chứa IBuoiKhamRepository
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Services.Implementations
{
    public class BuoiKhamService : IBuoiKhamService
    {
        private readonly IBuoiKhamRepository _repo;

        public BuoiKhamService(IBuoiKhamRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> DatLichKhamAsync(DatLichRequest request, int currentUserId, string role)
        {
            var buoiKham = new BuoiKham
            {
                Ngay = request.Ngay,
                Gio = request.Gio,
                BacSiId = request.BacSiId,
                PhongKhamId = request.PhongKhamId,
                TrangThai = TrangThaiBuoiKham.ChuaXacNhan
            };

            // LOGIC PHÂN QUYỀN Ở ĐÂY
            if (role == "BenhNhan")
            {
                // Bệnh nhân tự đặt -> Ép Id của chính họ
                buoiKham.BenhNhanId = currentUserId;
            }
            else if (role == "Admin" || role == "LeTan")
            {
                // Admin/Lễ tân đặt hộ -> Phải truyền Id bệnh nhân lên
                if (!request.BenhNhanId.HasValue || request.BenhNhanId.Value <= 0)
                    throw new Exception("Vui lòng chọn bệnh nhân để đặt lịch hộ.");

                buoiKham.BenhNhanId = request.BenhNhanId.Value;

                // Lễ tân đặt thì cho phép Confirm luôn
                buoiKham.TrangThai = TrangThaiBuoiKham.XacNhan;
            }
            else
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng này.");
            }

            return await _repo.AddAsync(buoiKham);
        }

        public async Task<bool> CapNhatTrangThaiAsync(int buoiKhamId, TrangThaiBuoiKham trangThaiMoi, int currentUserId, string role)
        {
            var buoiKham = await _repo.GetByIdAsync(buoiKhamId);
            if (buoiKham == null)
                throw new Exception("Không tìm thấy buổi khám.");

            // LOGIC PHÂN QUYỀN ĐỔI TRẠNG THÁI
            if (role == "BacSi")
            {
                // Bác sĩ chỉ xem & sửa lịch của mình
                if (buoiKham.BacSiId != currentUserId)
                    throw new UnauthorizedAccessException("Bạn không thể sửa lịch của bác sĩ khác.");
            }
            else if (role == "BenhNhan")
            {
                // Bệnh nhân chỉ được HỦY lịch của chính mình
                if (buoiKham.BenhNhanId != currentUserId || trangThaiMoi != TrangThaiBuoiKham.Huy)
                    throw new UnauthorizedAccessException("Bệnh nhân chỉ được phép tự hủy lịch của mình.");
            }
            else if (role != "Admin" && role != "LeTan")
            {
                throw new UnauthorizedAccessException("Tài khoản không hợp lệ.");
            }
            // Admin & Lễ tân đi qua hết các if trên -> có toàn quyền sửa

            buoiKham.TrangThai = trangThaiMoi;
            return await _repo.UpdateAsync(buoiKham);
        }
    }
}