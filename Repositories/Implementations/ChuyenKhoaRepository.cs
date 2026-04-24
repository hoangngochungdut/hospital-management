using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;

public class ChuyenKhoaRepository : IChuyenKhoaRepository
{
    private readonly AppDbContext _context;

    public ChuyenKhoaRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(ChuyenKhoa entity)
    {
        _context.ChuyenKhoas.Add(entity);
        _context.SaveChanges();
    }

    // Gộp GetAll: Giữ ICollection của bạn nhưng thêm OrderBy của master cho chuyên nghiệp
    public ICollection<ChuyenKhoa> GetAll()
    {
        return _context.ChuyenKhoas
            .OrderBy(x => x.TenKhoa) // Sắp xếp tên khoa từ A-Z theo ý của master
            .ToList(); 
    }

    public ChuyenKhoa? GetById(int id)
    {
        return _context.ChuyenKhoas.Find(id);
    }

    public void Update(ChuyenKhoa entity)
    {
        _context.ChuyenKhoas.Update(entity);
        _context.SaveChanges();
    }

    public void Delete(ChuyenKhoa entity)
    {
        _context.ChuyenKhoas.Remove(entity);
        _context.SaveChanges();
    }
}