using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class PhongKhamRepository : IPhongKhamRepository
    {
        private readonly AppDbContext _context;

        public PhongKhamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PhongKham>> GetByChuyenKhoaIdAsync(int chuyenKhoaId)
        {
            return await _context.PhongKhams
                .Where(p => p.ChuyenKhoaId == chuyenKhoaId)
                .ToListAsync();
        }
    }
}