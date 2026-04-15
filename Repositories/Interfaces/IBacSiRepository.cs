using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IBacSiRepository : IRepository<BacSi>
    {
        //BacSi? GetByNguoiDungId(int nguoiDungId);
        public XemHoSoBacSiResponse? GetHoSo(int id);
    }
}