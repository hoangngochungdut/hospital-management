using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IBacSiRepository : IRepository<BacSi>
    {
        // Thằng IRepository đã có: BacSi GetById(int id); rồi, không cần viết lại.

        public XemHoSoBacSiResponse? GetHoSo(int id);

        // Hàm này dùng Task vì nó là hàm mới, không bị ràng buộc bởi IRepository
        Task<List<BacSi>> GetByChuyenKhoaIdAsync(int chuyenKhoaId);
    }
}