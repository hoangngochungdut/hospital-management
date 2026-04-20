using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Implementations;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.BussinessValidationServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Implementations
{
    public class TaiKhoanService
    {
        ITaiKhoanRepository _repo;
        AccountValidationService _avservice;
        public TaiKhoanService (ITaiKhoanRepository repo, AccountValidationService avservice)
        {
            _repo = repo;
            _avservice = avservice;
        }

        public void Add(TaiKhoan taiKhoan)
        {
            if (_avservice.IsUsernameTaken(taiKhoan.TenDangNhap))
                throw new Exception("Username đã tồn tại");

            _repo.Add(taiKhoan);
        }

        public TaiKhoan? GetById(int id)
        {
            return _repo.GetById(id);
        }

        public ICollection<TaiKhoan> GetAll()
        {
            return _repo.GetAll();
        }

        public void Update(TaiKhoan taiKhoan)
        {
            if (!_avservice.TaiKhoanExisted(taiKhoan)) throw new Exception("Tài khoản không tồn tại");
            _repo.Update(taiKhoan);
        }

        public void Delete(TaiKhoan taiKhoan)
        {
            if (!_avservice.TaiKhoanExisted(taiKhoan)) throw new Exception("Tài khoản không tồn tại");
            _repo.Delete(taiKhoan);
        }

        public TaiKhoan? GetByUsername(string username)
        {
            return _repo.GetByUsername(username);
        }

        public bool ExistedByUsername(string username)
        {
            return _repo.ExistedByUsername(username);
        }
    }
}
