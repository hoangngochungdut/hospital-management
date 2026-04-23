using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Interfaces;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class BacSiRepository : IBacSiRepository
    {
        private readonly AppDbContext _context;

        public BacSiRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(BacSi entity)
        {
            _context.BacSis.Add(entity);
        }

        public ICollection<BacSi> GetAll()
        {
            return _context.BacSis
                .Include(x => x.ChuyenKhoa)
                .Include(x => x.BuoiKhams)
                .ToList();
        }

        public BacSi? GetById(int id)
        {
            return _context.BacSis.Find(id);
        }

        public void Update(BacSi entity)
        {
            _context.BacSis.Update(entity);
        }

        public void Delete(BacSi entity)
        {
            _context.BacSis.Remove(entity);
        }

        public XemHoSoBacSiResponse? GetHoSo(int id)
        {
            return _context.BacSis
                .Where(b => b.Id == id)
                .Select(b => new XemHoSoBacSiResponse
                {
                    HoTen = b.HoTen ?? "",
                    GioiTinh = b.GioiTinh,
                    DiaChi = b.DiaChi,
                    SoDienThoai = b.Sdt,
                    TenChuyenKhoa = b.ChuyenKhoa.TenKhoa
                })
                .FirstOrDefault();
        }
        public async Task<List<BacSi>> GetByChuyenKhoaIdAsync(int chuyenKhoaId)
        {
            return await _context.BacSis
                .Where(b => b.ChuyenKhoaId == chuyenKhoaId)
                .ToListAsync();
        }

    }
}