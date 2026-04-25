using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class NguoiDungRepository : INguoiDungRepository
    {
        private readonly AppDbContext _context;

        public NguoiDungRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(NguoiDung entity)
        {
            _context.NguoiDungs.Add(entity);
            _context.SaveChanges();
        }

        public ICollection<NguoiDung> GetAll()
        {
            return _context.NguoiDungs.ToList();
        }

        public NguoiDung? GetById(int id)
        {
            return _context.NguoiDungs.Find(id);
        }

        public void Update(NguoiDung entity)
        {
            _context.NguoiDungs.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(NguoiDung entity)
        {
            _context.NguoiDungs.Remove(entity);
            _context.SaveChanges();
        }

        public NguoiDung? GetByTaiKhoanId(int taiKhoanId)
        {
            return _context.NguoiDungs
                .Include(n => n.TaiKhoan)
                .FirstOrDefault(n => n.TaiKhoan != null && n.TaiKhoan.Id == taiKhoanId);
        }
    }
}