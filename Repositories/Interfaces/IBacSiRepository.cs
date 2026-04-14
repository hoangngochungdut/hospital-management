using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IBacSiRepository : IRepository<BacSi>
    {
        BacSi? GetByNguoiDungId(int nguoiDungId);
    }
}