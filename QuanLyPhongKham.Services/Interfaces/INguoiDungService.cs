using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface INguoiDungService
    {
        public bool ExistedByEmail(string email);
        public bool ExistedByPhoneNumber(string phonenumber);
    }
}
