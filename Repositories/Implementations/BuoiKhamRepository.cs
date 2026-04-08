using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data; // Import DbContext của bạn
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class BuoiKhamRepository : IBuoiKhamRepository
    {
        private readonly AppDbContext _context; // Đổi AppDbContext thành tên context của bạn nếu khác

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

        public async Task<BuoiKham> GetByIdAsync(int id)
        {
            return await _context.BuoiKhams.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateAsync(BuoiKham buoiKham)
        {
            _context.BuoiKhams.Update(buoiKham);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}