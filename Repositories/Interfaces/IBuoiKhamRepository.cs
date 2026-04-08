using System;
using System.Collections.Generic;
using System.Text;

using QuanLyPhongKham.Models; 
using System.Threading.Tasks;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IBuoiKhamRepository
    {
        Task<bool> AddAsync(BuoiKham buoiKham);
        Task<BuoiKham> GetByIdAsync(int id);
        Task<bool> UpdateAsync(BuoiKham buoiKham);
    }
}