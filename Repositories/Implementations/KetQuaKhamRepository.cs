using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

public class KetQuaKhamRepository : IKetQuaKhamRepository
{
    private readonly AppDbContext _context;

    public KetQuaKhamRepository(AppDbContext context)
    {
        _context = context;
    }

    public KetQuaKham? GetByBuoiKhamId(int buoiKhamId)
    {
        return _context.KetQuaKhams
            .FirstOrDefault(x => x.BuoiKhamId == buoiKhamId);
    }
}