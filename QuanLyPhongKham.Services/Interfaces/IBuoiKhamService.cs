using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IBuoiKhamService
    {
        Task<bool> DatLichKhamAsync(DatLichRequest request, int currentUserId, string role);
        Task<bool> CapNhatTrangThaiAsync(int buoiKhamId, TrangThaiBuoiKham trangThaiMoi, int currentUserId, string role);
        public List<BuoiKham> GetByBacSiId(int bacSiId);

        public List<BuoiKham> GetAllLichKham();
        public BuoiKham GetById(int id);

        public bool CapNhatTrangThai(int id, TrangThaiBuoiKham trangThaiMoi);
        Task<object> LayBacSiVaPhongTheoKhoaAsync(int chuyenKhoaId);
        Task<List<string>> LayCacGioKhamTrongAsync(int bacSiId, int phongKhamId, DateOnly ngay);
        Task<List<ChuyenKhoa>> LayTatCaChuyenKhoaAsync();
        Task<List<BenhNhan>> LayTatCaBenhNhanAsync();
    }
}