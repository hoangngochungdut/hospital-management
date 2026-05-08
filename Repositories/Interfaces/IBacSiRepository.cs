using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IBacSiRepository : IRepository<BacSi>
    {
        BacSi? GetByIdWithTaiKhoan(int id);
        ICollection<BacSi> GetAllWithTaiKhoan();
        //BacSi? GetByNguoiDungId(int nguoiDungId);
        public XemHoSoBacSiResponse? GetHoSo(int id);

        // Hàm này dùng Task vì nó là hàm mới, không bị ràng buộc bởi IRepository
        Task<List<BacSi>> GetByChuyenKhoaIdAsync(int chuyenKhoaId);
        List<BacSi> GetDanhSachBacSiKemDanhGia();

    }
}