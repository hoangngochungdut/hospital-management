using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;

public class TieuSuBenhAnRepository : ITieuSuBenhAnRepository
{
    private readonly AppDbContext _context;

    public TieuSuBenhAnRepository(AppDbContext context)
    {
        _context = context;
    }

    public TieuSuBenhAn? GetByBenhNhanId(int benhNhanId)
    {
        return _context.TieuSuBenhAns
            .FirstOrDefault(x => x.BenhNhanId == benhNhanId);
    }
}