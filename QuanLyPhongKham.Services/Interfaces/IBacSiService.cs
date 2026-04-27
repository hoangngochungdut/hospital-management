using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IBacSiService
    {
        ICollection<BacSi> GetAll();
        XemHoSoBacSiResponse? GetHoSo(int nguoiDungId);

        // Trả về kiểu đồng bộ như cũ để không lỗi dây chuyền
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBacSiRequest request);

        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);

        // ✅ SỬA LẠI ĐÂY: Bỏ Task, bỏ async. Trả về BacSi thường.
        BacSi GetById(int id);

        ICollection<ChuyenKhoa> GetDanhSachChuyenKhoa();
        Task<IEnumerable<BacSi>> GetByChuyenKhoaIdAsync(int chuyenKhoaId);
        List<BacSiDanhGiaViewModel> LayDanhSachBacSiVaDanhGia();
    }
}