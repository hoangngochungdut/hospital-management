using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Implementations;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.BussinessValidationServices;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace QuanLyPhongKham.Services.Implementations
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly AccountValidationService _avservice;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IEmailSender _emailSender;
        //private readonly SmsService _smsService;
        public TaiKhoanService (
            ITaiKhoanRepository repo, 
            AccountValidationService avservice,
             IEmailSender emailSender,
            IPasswordGenerator passwordGenerator)
        {
            _taiKhoanRepository = repo;
            _avservice = avservice;
            _emailSender = emailSender;
            _passwordGenerator = passwordGenerator;
            //_smsService = smsService;
        }

        public void Add(TaiKhoan taiKhoan)
        {
            if (_avservice.IsUsernameTaken(taiKhoan.TenDangNhap))
                throw new Exception("Username đã tồn tại");

            _taiKhoanRepository.Add(taiKhoan);
        }

        public TaiKhoan? GetById(int id)
        {
            return _taiKhoanRepository.GetById(id);
        }

        public ICollection<TaiKhoan> GetAll()
        {
            return _taiKhoanRepository.GetAll();
        }

        public void Update(TaiKhoan taiKhoan)
        {
            if (!_avservice.TaiKhoanExisted(taiKhoan)) throw new Exception("Tài khoản không tồn tại");
            _taiKhoanRepository.Update(taiKhoan);
        }

        public void Delete(TaiKhoan taiKhoan)
        {
            if (!_avservice.TaiKhoanExisted(taiKhoan)) throw new Exception("Tài khoản không tồn tại");
            _taiKhoanRepository.Delete(taiKhoan);
        }

        public TaiKhoan? GetByUsername(string username)
        {
            return _taiKhoanRepository.GetByUsername(username);
        }

        

        public async Task ResetMatKhau(int id)
        {
            //Console.WriteLine("Di vao reset mat khau");

            string tempPassword = _passwordGenerator.Generate(10);

            var taiKhoan = _taiKhoanRepository.GetByIdWithNguoiDung(id);
            if (taiKhoan == null)
                throw new Exception("Tài khoản không tồn tại");

            taiKhoan.MatKhauHash = tempPassword;
            taiKhoan.IsMustChangePassword = true;
            _taiKhoanRepository.Update(taiKhoan);

            Console.WriteLine("Email: " + taiKhoan.NguoiDung.Email);
            await _emailSender.SendResetPasswordEmail(
                taiKhoan.NguoiDung.Email,
                tempPassword
            );

        }
        public bool ExistedByUsername(string username)
        {
            return _taiKhoanRepository.ExistedByUsername(username);
        }

        public TaiKhoan? GetByUsernameWithNguoiDung(string username)
        {
            return _taiKhoanRepository.GetByUsernameWithNguoiDung(username);
        }
    }
}
