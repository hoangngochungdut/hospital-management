using QuanLyPhongKham.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IPhongKhamRepository
    {
        Task<IEnumerable<PhongKham>> GetByChuyenKhoaAsync(int chuyenKhoaId);
        Task<IEnumerable<PhongKham>> GetAllAsync(); // Tiện tay làm luôn hàm lấy tất cả
    }
}