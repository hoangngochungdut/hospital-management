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

        public async Task<List<BuoiKham>> GetCacCaDaDatAsync(DateOnly ngay, TimeOnly gio)
        {
            return await _context.BuoiKhams
                .Where(b => b.Ngay == ngay
                         && b.Gio == gio
                         && (b.TrangThai == TrangThaiBuoiKham.XacNhan || b.TrangThai == TrangThaiBuoiKham.ChuaXacNhan))
                .ToListAsync();
        }
        public async Task<List<BuoiKham>> GetLichDaDatTrongNgayAsync(DateOnly ngay, int bacSiId, int phongKhamId)
        {
            return await _context.BuoiKhams
                .Where(b => b.Ngay == ngay
                         && (b.BacSiId == bacSiId || b.PhongKhamId == phongKhamId)
                         && (b.TrangThai == Models.Enums.TrangThaiBuoiKham.XacNhan ||
                             b.TrangThai == Models.Enums.TrangThaiBuoiKham.ChuaXacNhan))
                .ToListAsync();
        }
        public List<BuoiKham> GetByBenhNhanId(int benhNhanId)
        {
            return _context.BuoiKhams
                .Include(b => b.BacSi)
                .Include(b => b.PhongKham)
                .Include(b => b.BacSi) // Tải thông tin Bác sĩ
                .ThenInclude(bs => bs.ChuyenKhoa)
                .Where(b => b.BenhNhanId == benhNhanId)
                .OrderByDescending(b => b.Ngay)
                .ThenByDescending(b => b.Gio)
                .ToList();
        }

        public bool Update(BuoiKham buoiKham)
        {
            _context.BuoiKhams.Update(buoiKham);
            var result = _context.SaveChanges();
            return result > 0;
        }

        public bool Delete(int id)
        {
            var lich = _context.BuoiKhams.Find(id);
            if (lich != null)
            {
                _context.BuoiKhams.Remove(lich);
                var result = _context.SaveChanges();
                return result > 0;
            }
            return false;
        }

        public async Task<List<TimeOnly>> GetCacGioDaDatAsync(int bacSiId, int phongKhamId, DateOnly ngayKham)
        {
            return await _context.BuoiKhams
                .Where(b => b.BacSiId == bacSiId
                         && b.PhongKhamId == phongKhamId
                         && b.Ngay == ngayKham
                         && b.TrangThai != TrangThaiBuoiKham.Huy)
                .Select(b => b.Gio)
                .ToListAsync();
        }
        public async Task<List<BuoiKham>> GetAllLichKhamFullAsync()
        {
            return await _context.BuoiKhams
                .Include(b => b.BenhNhan)
                .Include(b => b.PhongKham)
                .Include(b => b.BacSi)
                    .ThenInclude(bs => bs.ChuyenKhoa) // <-- THÊM DÒNG NÀY ĐỂ LẤY TÊN KHOA
                .OrderByDescending(b => b.Ngay)
                .ToListAsync();
        }
    }
}