using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IBacSiService
    {
        ICollection<BacSi> GetAll();
        BacSi? GetById(int id);
        BacSi? GetByIdWithTaiKhoan(int id);
        XemHoSoBacSiResponse? GetHoSo(int nguoiDungId);

        // Trả về kiểu đồng bộ như cũ để không lỗi dây chuyền
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBacSiRequest request);

        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);
        void Add(AddBacSiDto entity);
        void Update(int id, CapNhatHoSoBacSiRequest bacsi);
        void Delete(int id);
        public List<BacSiDanhGiaViewModel> LayDanhSachBacSiVaDanhGia();
        public Task<IEnumerable<BacSi>> GetByChuyenKhoaIdAsync(int chuyenKhoaId);

    }
}