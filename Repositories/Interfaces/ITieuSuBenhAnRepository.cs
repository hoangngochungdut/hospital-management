using QuanLyPhongKham.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public  interface  ITieuSuBenhAnRepository
    {
        TieuSuBenhAn? GetByBenhNhanId(int benhNhanId);
    }
}
