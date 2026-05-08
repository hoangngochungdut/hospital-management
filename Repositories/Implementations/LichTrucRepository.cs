using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class LichTrucRepository : ILichTrucRepository
    {
        private readonly AppDbContext _context;

        public LichTrucRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LichTruc>> GetLichByChuyenKhoaAsync(int chuyenKhoaId, DateOnly homNay)
        {
            return await _context.LichTrucs
                .Include(lt => lt.BacSi)
                .Include(lt => lt.PhongKham)
                .Where(lt => lt.BacSi.ChuyenKhoaId == chuyenKhoaId && lt.Ngay >= homNay)
                .ToListAsync();
        }

        public async Task<bool> ThemLichPhanCongAsync(LichTruc lichTruc)
        {
            try
            {
                _context.LichTrucs.Add(lichTruc);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> KiemTraTrungLichAsync(int bacSiId, int phongKhamId, DateOnly ngay)
        {
            // Check 2 điều kiện: 
            // 1. Bác sĩ này ngày đó đã trực ở đâu chưa?
            // 2. PHÒNG này ngày đó đã có bác sĩ nào khác trực chưa?
            return await _context.LichTrucs
                .AnyAsync(lt => (lt.BacSiId == bacSiId && lt.Ngay == ngay)
                             || (lt.PhongKhamId == phongKhamId && lt.Ngay == ngay));
        }

        // ĐÃ NÂNG CẤP: Kéo theo Chuyên Khoa để hiển thị ra UI
        public async Task<IEnumerable<LichTruc>> LayTatCaLichTrucAsync()
        {
            return await _context.LichTrucs
                .Include(l => l.BacSi)
                    .ThenInclude(b => b.ChuyenKhoa) // 👉 Móc sang bảng Khoa ở đây nè!
                .Include(l => l.PhongKham)
                .OrderByDescending(l => l.Ngay)
                .ToListAsync();
        }

        public async Task<bool> ThemDanhSachLichTrucAsync(List<LichTruc> dsLich)
        {
            try
            {
                await _context.LichTrucs.AddRangeAsync(dsLich);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 👉 ĐÃ THÊM HÀM XÓA: Phục vụ cho cái nút Xóa màu đỏ ở UI
        public async Task<bool> XoaLichTrucAsync(int id)
        {
            var lich = await _context.LichTrucs.FindAsync(id);
            if (lich == null) return false; // Không tìm thấy thì báo false

            _context.LichTrucs.Remove(lich);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<LichKhaDungDto>> GetLichKhaDungByKhoaAsync(int chuyenKhoaId, DateOnly tuNgay)
        {
            return await _context.LichTrucs
                .Include(l => l.BacSi)
                .Include(l => l.PhongKham)
                .Where(l => l.BacSi.ChuyenKhoaId == chuyenKhoaId && l.Ngay >= tuNgay)
                .Select(l => new LichKhaDungDto
                {
                    BacSiId = l.BacSiId,
                    BacSiTen = "BS. " + l.BacSi.HoTen,
                    PhongId = l.PhongKhamId,
                    PhongTen = "Phòng " + l.PhongKham.SoPhong,
                    LoaiPhong = l.PhongKham.LoaiPhong,
                    Ngay = l.Ngay.ToString("yyyy-MM-dd")
                })
                .ToListAsync();
        }
        public async Task<List<LichTruc>> GetLichTrucTuNgayAsync(int bacSiId, DateOnly tuNgay)
        {
            return await _context.LichTrucs
                           .Include(l => l.PhongKham) // Nhớ Include để View lấy được Số phòng
                           .Where(l => l.BacSiId == bacSiId && l.Ngay >= tuNgay)
                           .OrderBy(l => l.Ngay)
                           .ToListAsync();
        }
        public async Task<bool> XoaNhieuLichTrucAsync(List<int> ids)
        {
            var lichTrucs = await _context.LichTrucs.Where(l => ids.Contains(l.Id)).ToListAsync();
            if (!lichTrucs.Any()) return false;

            _context.LichTrucs.RemoveRange(lichTrucs);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}