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
        Task<BuoiKham?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(BuoiKham buoiKham);

        List<BuoiKham> GetByBacSiId(int bacSiId);
        List<BuoiKham> GetAll();
        BuoiKham? GetById(int id);
        // Xóa dòng cũ đi và thay bằng dòng này:
        Task<List<BuoiKham>> GetCacCaDaDatAsync(DateOnly ngay, TimeOnly gio);
        Task<List<BuoiKham>> GetLichDaDatTrongNgayAsync(DateOnly ngay, int bacSiId, int phongKhamId);
    }
}