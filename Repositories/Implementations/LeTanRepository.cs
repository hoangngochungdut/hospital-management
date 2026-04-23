using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class LeTanRepository : ILeTanRepository
    {
        private readonly AppDbContext _context;

        public LeTanRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(LeTan entity)
        {
            _context.LeTans.Add(entity);
            _context.SaveChanges();
        }

        public ICollection<LeTan> GetAll()
        {
            return _context.LeTans.ToList();
        }

        public LeTan? GetById(int id)
        {
            return _context.LeTans.Find(id);
        }

        public LeTan? GetByNguoiDungId(int nguoiDungId)
        {
            return _context.LeTans.FirstOrDefault(x => x.Id == nguoiDungId);
        }

        public void Update(LeTan entity)
        {
            _context.LeTans.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(LeTan entity)
        {
            _context.LeTans.Remove(entity);
            _context.SaveChanges();
        }

    }
}