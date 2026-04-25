using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            _context.SaveChanges();
        }

        public ICollection<BacSi> GetAll()
        {
            return _context.BacSis
                .Include(x => x.ChuyenKhoa)
                .Include(x => x.BuoiKhams)
                .ToList();
        }

        // 👉 FIX LỖI CS0738: Trả về BacSi thường, không dùng Task ở đây để khớp Interface của nhóm
        public BacSi GetById(int id)
        {
            return _context.BacSis
                .Include(b => b.ChuyenKhoa) // Nạp Chuyên Khoa để Controller không bị Null TenKhoa
                .FirstOrDefault(b => b.Id == id);
        }

        public void Update(BacSi entity)
        {
            _context.BacSis.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(BacSi entity)
        {
            _context.BacSis.Remove(entity);
            _context.SaveChanges();
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

        // Hàm này nếu Interface của nhóm cho phép dùng Async thì giữ nguyên
        public async Task<List<BacSi>> GetByChuyenKhoaIdAsync(int chuyenKhoaId)
        {
            return await _context.BacSis
                .Where(b => b.ChuyenKhoaId == chuyenKhoaId)
                .Include(b => b.ChuyenKhoa)
                .ToListAsync();
        }
    }
}