using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Implementations
{
    public class NguoiDungService : INguoiDungService
    {
        private readonly INguoiDungRepository _nguoiDungRepository;

        public NguoiDungService(INguoiDungRepository nguoiDungRepository)
        {
            _nguoiDungRepository = nguoiDungRepository;
        }
        public bool ExistedByEmail(string email)
        {
            return _nguoiDungRepository.ExistedByEmail(email);
        }

        public bool ExistedByPhoneNumber(string phonenumber)
        {
            return _nguoiDungRepository.ExistedByPhoneNumber(phonenumber);
        }
    }
}
