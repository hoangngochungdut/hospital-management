using QuanLyPhongKham.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface ITaiKhoanService
    {
        public void Add(TaiKhoan taiKhoan);
        public TaiKhoan? GetById(int id);
        public ICollection<TaiKhoan> GetAll();
        public void Update(TaiKhoan taiKhoan);
        public void Delete(TaiKhoan taiKhoan);
        public TaiKhoan? GetByUsername(string username);
        public TaiKhoan? GetByUsernameWithNguoiDung(string username);
        public bool ExistedByUsername(string username);
        public Task ResetMatKhau(int id);
    }
}
