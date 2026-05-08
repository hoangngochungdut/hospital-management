using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data; // Phải có cái này để gọi AppDbContext
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces; // Phải có cái này để gọi IBenhNhanRepository
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class BenhNhanRepository : IBenhNhanRepository
    {
        private readonly AppDbContext _context;

        public BenhNhanRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(BenhNhan entity)
        {
            _context.BenhNhans.Add(entity);
            _context.SaveChanges();
        }

        public ICollection<BenhNhan> GetAll()
        {
            return _context.BenhNhans
                .ToList();
        }

        public ICollection<BenhNhan> GetAllWithTaiKhoan()
        {
            return _context.BenhNhans
                .Include(x => x.TaiKhoan)
                .ToList();
        }

        public async Task<List<BenhNhan>> GetAllAsync()
        {
            return await _context.BenhNhans.ToListAsync();
        }

        public BenhNhan? GetById(int id)
        {
            return _context.BenhNhans
                .FirstOrDefault(x => x.Id == id);
        }

        public void Update(BenhNhan entity)
        {
            _context.BenhNhans.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(BenhNhan entity)
        {
            _context.BenhNhans.Remove(entity);
            _context.SaveChanges();

        }
        public BenhNhan? GetByIdWithTaiKhoan(int id)
        {
            return _context.BenhNhans
                .Include(x => x.TaiKhoan)
                .FirstOrDefault(x => x.Id == id);
        }

    }
}