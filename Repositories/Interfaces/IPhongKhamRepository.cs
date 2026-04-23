using QuanLyPhongKham.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IPhongKhamRepository
    {
        Task<List<PhongKham>> GetByChuyenKhoaIdAsync(int chuyenKhoaId);
    }
}
