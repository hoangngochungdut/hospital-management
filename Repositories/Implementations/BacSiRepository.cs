using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;

namespace QuanLyPhongKham.Repositories.Implementations
{
    public class BacSiRepository : IBacSiRepository
    {
        private readonly AppDbContext _context;

        public BacSiRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(BacSi entity)
        {
            _context.BacSis.Add(entity);
            _context.SaveChanges();
        }

        public ICollection<BacSi> GetAll()
        {
            return _context.BacSis.ToList();
        }

        public BacSi? GetById(int id)
        {
            return _context.BacSis.Find(id);
        }

        public BacSi? GetByNguoiDungId(int nguoiDungId)
        {
            return _context.BacSis.FirstOrDefault(x => x.NguoiDungId == nguoiDungId);
        }

        public void Update(BacSi entity)
        {
            _context.BacSis.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(BacSi entity)
        {
            _context.BacSis.Remove(entity);
            _context.SaveChanges();
        }
    }
}