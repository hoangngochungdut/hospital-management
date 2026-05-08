using QuanLyPhongKham.Data; // Đổi lại namespace DbContext của ông nếu khác
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class KetQuaKhamRepository : IKetQuaKhamRepository
    {
        private readonly AppDbContext _context; // Hoặc tên DbContext mà ông đang dùng

        public KetQuaKhamRepository(AppDbContext context)
        {
            _context = context;
        }

        public KetQuaKham GetById(int buoiKhamId)
        {
            // Vì BuoiKhamId là khóa chính nên dùng Find hoặc FirstOrDefault đều được
            return _context.KetQuaKhams.FirstOrDefault(k => k.BuoiKhamId == buoiKhamId);
        }

        public bool Add(KetQuaKham entity)
        {
            _context.KetQuaKhams.Add(entity);
            return _context.SaveChanges() > 0;
        }

        public bool Update(KetQuaKham entity)
        {
            _context.KetQuaKhams.Update(entity);
            return _context.SaveChanges() > 0;
        }
    }
}