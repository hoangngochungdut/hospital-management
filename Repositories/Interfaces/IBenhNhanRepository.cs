using QuanLyPhongKham.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IBenhNhanRepository 
    {
       Task<List<BenhNhan>> GetAllAsync();
        
        public void Add(BenhNhan entity);
        public void Delete(BenhNhan entity);
        ICollection<BenhNhan> GetAll();
        public ICollection<BenhNhan> GetAllWithTaiKhoan();
        public BenhNhan? GetById(int id);
        public BenhNhan? GetByIdWithTaiKhoan(int id);
        public void Update(BenhNhan entity);


    }
}
