using System;
using System.Collections.Generic;
using System.Text;
using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public  interface INguoiDungRepository : IRepository<NguoiDung>
    {
        NguoiDung? GetByTaiKhoanId(int taiKhoanId);
    }
}
