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
            throw new NotImplementedException();
        }

        public void Delete(BenhNhan entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<BenhNhan> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<List<BenhNhan>> GetAllAsync()
        {
            return await _context.BenhNhans.ToListAsync();
        }

        public BenhNhan? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(BenhNhan entity)
        {
            throw new NotImplementedException();
        }
       
    }
}