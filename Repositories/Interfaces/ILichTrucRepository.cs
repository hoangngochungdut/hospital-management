using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface ILichTrucRepository
    {
        Task<IEnumerable<LichTruc>> GetLichByChuyenKhoaAsync(int chuyenKhoaId, DateOnly homNay);
        Task<bool> ThemLichPhanCongAsync(LichTruc lichTruc);
        Task<bool> KiemTraTrungLichAsync(int bacSiId, int phongKhamId, DateOnly ngay);
        Task<IEnumerable<LichTruc>> LayTatCaLichTrucAsync();
        Task<bool> ThemDanhSachLichTrucAsync(List<LichTruc> dsLich);
        Task<bool> XoaLichTrucAsync(int id);
        Task<List<LichKhaDungDto>> GetLichKhaDungByKhoaAsync(int chuyenKhoaId, DateOnly tuNgay);
        Task<List<LichTruc>> GetLichTrucTuNgayAsync(int bacSiId, DateOnly tuNgay);
        Task<bool> XoaNhieuLichTrucAsync(List<int> ids);
    }
}