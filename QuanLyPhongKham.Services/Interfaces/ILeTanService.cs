using QuanLyPhongKham.Models.DTOs;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface ILeTanService
    {
        XemHoSoLeTanResponse? GetHoSo(int nguoiDungId);
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoLeTanRequest request);
        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);
     //   Task<IEnumerable<object>> SearchBenhNhanAsync(string term);
    }
}