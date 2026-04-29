using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface ILeTanService
    {
        // --- CÁC HÀM DÀNH CHO LỄ TÂN  ---
        XemHoSoLeTanResponse? GetHoSo(int nguoiDungId);
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoLeTanRequest request);
        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);
        Task<IEnumerable<BuoiKham>> GetDanhSachChoThanhToanAsync();
        Task<bool> XacNhanThanhToanAsync(int buoiKhamId, decimal tongTien);
        Task<IEnumerable<BenhNhan>> TimKiemBenhNhanAsync(string keyword);
        Task<BenhNhan?> GetChiTietBenhNhanAsync(int id);

        // --- CÁC HÀM BỔ SUNG CHO ADMIN QUẢN LÝ LỄ TÂN ---

        // 1. Lấy danh sách tất cả lễ tân
        Task<IEnumerable<LeTan>> GetAllLeTanAsync();

  
        Task<LeTan?> GetByIdAsync(int id);

        // 3. Thêm mới Lễ tân
        Task<(bool Success, string Message)> CreateLeTanAsync(LeTan leTan, string tenDangNhap, string matKhau);

        // 4. Cập nhật thông tin Lễ tân 
     
        Task<(bool Success, string Message)> UpdateLeTanAsync(LeTan leTan);

        // 5. Xóa tài khoản lễ tân
        // Sửa từ bool sang Task<(bool Success, string Message)> để Controller gọi được .Success
        Task<(bool Success, string Message)> DeleteLeTanAsync(int id);

        // 6. Tìm kiếm lễ tân
        Task<IEnumerable<LeTan>> SearchLeTanAsync(string keyword);
    }
}