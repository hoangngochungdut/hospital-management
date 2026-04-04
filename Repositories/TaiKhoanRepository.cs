using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace QuanLyPhongKham.Repositories
{
    public class TaiKhoanRepository : IRepository<TaiKhoan>
    {
        private readonly AppDbContext _context;
        public TaiKhoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(TaiKhoan taiKhoan)
        {
            _context.TaiKhoans.Add(taiKhoan);
            _context.SaveChanges();
        }

        public ICollection<TaiKhoan> GetAll()
        {
            return new Collection<TaiKhoan>(_context.TaiKhoans.ToList());
        }

        public TaiKhoan? GetById(int id)
        {
            TaiKhoan? taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Id == id);
            return taiKhoan;
        }
        public void Update(TaiKhoan taiKhoan)
        {
            _context.TaiKhoans.Update(taiKhoan); 
            _context.SaveChanges();
        }
        public void Delete(TaiKhoan taiKhoan)
        {
            _context.TaiKhoans.Remove(taiKhoan); 
            _context.SaveChanges(); 
        }
        public TaiKhoan? GetByUsername(string username)
        {
            TaiKhoan? taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == username);
            return taiKhoan;
        }
        public bool ExistedByUsername(string username)
        {
            return _context.TaiKhoans.Any(t => t.TenDangNhap == username);
        }


    }
}
