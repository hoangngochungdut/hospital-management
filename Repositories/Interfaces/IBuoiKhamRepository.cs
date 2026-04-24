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
        Task<List<BuoiKham>> GetCacCaDaDatAsync(DateOnly ngay, TimeOnly gio);
        Task<List<BuoiKham>> GetLichDaDatTrongNgayAsync(DateOnly ngay, int bacSiId, int phongKhamId);
        List<BuoiKham> GetByBenhNhanId(int benhNhanId);
        bool Update(BuoiKham buoiKham); 
        bool Delete(int id);
        Task<List<TimeOnly>> GetCacGioDaDatAsync(int bacSiId, int phongKhamId, DateOnly ngayKham);
    }
}
