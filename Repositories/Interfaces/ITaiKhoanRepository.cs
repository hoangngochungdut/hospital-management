using QuanLyPhongKham.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface ITaiKhoanRepository : IRepository<TaiKhoan>
    {
        public TaiKhoan? GetByUsername(string username);
        public bool ExistedByUsername(string username);
    }
}
