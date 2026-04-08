using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IBuoiKhamService
    {
        Task<bool> DatLichKhamAsync(DatLichRequest request, int currentUserId, string role);
        Task<bool> CapNhatTrangThaiAsync(int buoiKhamId, TrangThaiBuoiKham trangThaiMoi, int currentUserId, string role);
    }
}