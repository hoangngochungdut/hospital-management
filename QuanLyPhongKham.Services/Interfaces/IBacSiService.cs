using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IBacSiService
    {
        ICollection<BacSi> GetAll();
        XemHoSoBacSiResponse? GetHoSo(int nguoiDungId);
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBacSiRequest request);
        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);
        public BacSi GetById(int id);
        ICollection<ChuyenKhoa> GetDanhSachChuyenKhoa();

    }
}