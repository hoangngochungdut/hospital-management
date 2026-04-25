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

        public async Task<IEnumerable<PhongKham>> GetByChuyenKhoaAsync(int chuyenKhoaId)
        {
            return await _context.PhongKhams
                .Where(p => p.ChuyenKhoaId == chuyenKhoaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhongKham>> GetAllAsync()
        {
            return await _context.PhongKhams.ToListAsync();
        }
    }
}