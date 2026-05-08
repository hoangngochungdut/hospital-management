using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface ILichTrucService
    {
        Task<bool> PhanCongBacSiAsync(int bacSiId, int phongKhamId, DateOnly ngay);
        Task<IEnumerable<LichTruc>> LayTatCaLichTrucAsync();
        Task<bool> PhanCongNhieuNgayAsync(int bacSiId, int phongKhamId, List<DateOnly> danhSachNgay);
        Task<bool> XoaLichTrucAsync(int id);
        Task<List<LichKhaDungDto>> LayDanhSachLichKhaDungAsync(int chuyenKhoaId);
        Task<List<LichTruc>> LayLichTrucTuHomNayAsync(int bacSiId);
        Task<bool> XoaLichTheoDanhSachIdAsync(List<int> ids);
    }
}