using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Implementations;
using QuanLyPhongKham.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.BussinessValidationServices
{
    public class AccountValidationService
    {
        private readonly ITaiKhoanRepository _repo;
        public AccountValidationService(ITaiKhoanRepository repo)
        {
            _repo = repo;
        }

        public bool TaiKhoanExisted(TaiKhoan tk)
        {
            return _repo.ExistedByUsername(tk.TenDangNhap);
        }
        public bool IsMatchedPassword(string password, string username)
        {
            TaiKhoan? tk = _repo.GetByUsername(username);
            if (tk == null) return false;
            if (tk.MatKhauHash == password) return true;
            return false;
        }
        public bool IsUsernameTaken(string username)
        {
            return (_repo.GetByUsername(username) != null);
        }

    }
}
