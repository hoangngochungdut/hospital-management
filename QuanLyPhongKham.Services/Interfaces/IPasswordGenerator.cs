using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IPasswordGenerator
    {
        public string Generate(int length);
    }
}
