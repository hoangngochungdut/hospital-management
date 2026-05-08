using QuanLyPhongKham.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface ITaiKhoanRepository : IRepository<TaiKhoan>
    {
        public TaiKhoan? GetByUsername(string username);
        public TaiKhoan? GetByUsernameWithNguoiDung(string username);
        public TaiKhoan? GetByIdWithNguoiDung(int id);
        public bool ExistedByUsername(string username);
        TaiKhoan? GetByNguoiDungId(int nguoiDungId);
    }
}
