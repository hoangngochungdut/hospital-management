using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IBacSiRepository : IRepository<BacSi>
    {
        //BacSi? GetByNguoiDungId(int nguoiDungId);
        public XemHoSoBacSiResponse? GetHoSo(int id);
        // của đặt lịch khám, có thể lấy thông tin bác sĩ để hiển thị trong lịch sử đặt lịch khám của bệnh nhân
        Task<List<BacSi>> GetByChuyenKhoaIdAsync(int chuyenKhoaId);
    }
}