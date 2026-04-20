using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class BuoiKhamRepository : IBuoiKhamRepository
    {
        private readonly AppDbContext _context;

        public BuoiKhamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(BuoiKham buoiKham)
        {
            await _context.BuoiKhams.AddAsync(buoiKham);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<BuoiKham?> GetByIdAsync(int id)
        {
            return await _context.BuoiKhams.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateAsync(BuoiKham buoiKham)
        {
            _context.BuoiKhams.Update(buoiKham);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public List<BuoiKham> GetByBacSiId(int bacSiId)
        {
            return _context.BuoiKhams
                .Include(b => b.BacSi)
                .Include(b => b.BenhNhan)
                .Include(b => b.PhongKham)
                .Where(b => b.BacSiId == bacSiId)
                .OrderByDescending(b => b.Ngay)
                .ThenBy(b => b.Gio)
                .ToList();
        }

        public List<BuoiKham> GetAll()
        {
            return _context.BuoiKhams
                .Include(b => b.BacSi)
                .Include(b => b.BenhNhan)
                .Include(b => b.PhongKham)
                .OrderByDescending(b => b.Ngay)
                .ThenBy(b => b.Gio)
                .ToList();
        }

        public BuoiKham? GetById(int id)
        {
            return _context.BuoiKhams.Find(id);
        }

        // =====================================================================
        // HÀM ĐÁP ỨNG YÊU CẦU: LẤY CÁC CA ĐÃ ĐẶT ĐỂ CHECK TRÙNG LỊCH
        // =====================================================================
        public async Task<List<BuoiKham>> GetCacCaDaDatAsync(DateOnly ngay, TimeOnly gio)
        {
            return await _context.BuoiKhams
                .Where(b => b.Ngay == ngay
                         && b.Gio == gio
                         // LƯU Ý Ở ĐÂY: Ông gõ chữ TrangThaiBuoiKham. rồi chọn đúng tên Trạng Thái trong Enum của ông nhé
                         && (b.TrangThai == TrangThaiBuoiKham.XacNhan || b.TrangThai == TrangThaiBuoiKham.ChuaXacNhan))
                .ToListAsync();
        }
        public async Task<List<BuoiKham>> GetLichDaDatTrongNgayAsync(DateOnly ngay, int bacSiId, int phongKhamId)
        {
            // Lấy tất cả các ca đang Active của Bác sĩ HOẶC Phòng đó trong 1 ngày cụ thể
            return await _context.BuoiKhams
                .Where(b => b.Ngay == ngay
                         && (b.BacSiId == bacSiId || b.PhongKhamId == phongKhamId)
                         && (b.TrangThai == Models.Enums.TrangThaiBuoiKham.XacNhan ||
                             b.TrangThai == Models.Enums.TrangThaiBuoiKham.ChuaXacNhan))
                .ToListAsync();
        }
    }
}