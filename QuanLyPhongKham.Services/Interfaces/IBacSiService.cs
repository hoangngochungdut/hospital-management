using QuanLyPhongKham.Models.DTOs;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IBacSiService
    {
        XemHoSoBacSiResponse? GetHoSo(int nguoiDungId);
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBacSiRequest request);
        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);
    }
}