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

    public async Task<List<ChuyenKhoa>> GetAllAsync()
    {
        return await _context.ChuyenKhoas
            .OrderBy(x => x.TenKhoa)
            .ToListAsync();
    }
}