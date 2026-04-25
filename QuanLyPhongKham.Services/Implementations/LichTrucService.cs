using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Implementations
{
    public class LichTrucService : ILichTrucService
    {
        private readonly ILichTrucRepository _lichTrucRepo;

        public LichTrucService(ILichTrucRepository lichTrucRepo)
        {
            _lichTrucRepo = lichTrucRepo;
        }

        public async Task<bool> PhanCongBacSiAsync(int bacSiId, int phongKhamId, DateOnly ngay)
        {
            // Cập nhật: Truyền thêm phongKhamId để chặn trùng cả phòng
            bool daCoLich = await _lichTrucRepo.KiemTraTrungLichAsync(bacSiId, phongKhamId, ngay);
            if (daCoLich) throw new Exception("Bác sĩ đã có lịch hoặc Phòng này đã có người trực vào ngày này!");

            var lichMoi = new LichTruc
            {
                BacSiId = bacSiId,
                PhongKhamId = phongKhamId,
                Ngay = ngay
            };

            return await _lichTrucRepo.ThemLichPhanCongAsync(lichMoi);
        }

        public async Task<IEnumerable<LichTruc>> LayTatCaLichTrucAsync()
        {
            return await _lichTrucRepo.LayTatCaLichTrucAsync();
        }

        public async Task<bool> PhanCongNhieuNgayAsync(int bacSiId, int phongKhamId, List<DateOnly> danhSachNgay)
        {
            var dsLichMoi = new List<LichTruc>();

            foreach (var date in danhSachNgay)
            {
                // CẬP NHẬT QUAN TRỌNG: Check trùng cả Bác sĩ và Phòng
                bool daCo = await _lichTrucRepo.KiemTraTrungLichAsync(bacSiId, phongKhamId, date);

                if (!daCo)
                {
                    dsLichMoi.Add(new LichTruc
                    {
                        BacSiId = bacSiId,
                        PhongKhamId = phongKhamId,
                        Ngay = date
                    });
                }
            }

            if (dsLichMoi.Any())
            {
                return await _lichTrucRepo.ThemDanhSachLichTrucAsync(dsLichMoi);
            }

            return false; // Trả về false nếu tất cả ngày chọn đều bị trùng
        }

        public async Task<bool> XoaLichTrucAsync(int id)
        {
            return await _lichTrucRepo.XoaLichTrucAsync(id);
        }
    }
}