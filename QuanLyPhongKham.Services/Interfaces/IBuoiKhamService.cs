using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IBuoiKhamService
    {
        // --- QUẢN LÝ DANH SÁCH (READ) ---

        // Hàm trọng tâm cho Admin quản lý tổng thể
        // Thêm 2 tham số lọc vào đây
        Task<List<BuoiKham>> LayToanBoLichKhamAdminAsync(int? chuyenKhoaId = null, string? tenBacSi = null);

        List<BuoiKham> GetByBacSiId(int bacSiId);
        List<BuoiKham> GetByBenhNhanId(int benhNhanId);
        BuoiKham GetById(int id);
        Task<List<ChuyenKhoa>> LayTatCaChuyenKhoaAsync();


        // --- NGHIỆP VỤ ĐẶT LỊCH ---
        Task<object> LayBacSiVaPhongTheoKhoaAsync(int chuyenKhoaId);
        Task<List<string>> LayCacGioKhamTrongAsync(int bacSiId, int phongKhamId, DateOnly ngay);
        Task<bool> DatLichKhamAsync(DatLichRequest request, int currentUserId, string role);


        // --- LUỒNG TRẠNG THÁI & DỜI LỊCH ---

        // Hàm dùng chung cho Admin, Bác sĩ để cập nhật trạng thái (Xác nhận, Hoàn thành, Hủy)
        bool CapNhatTrangThai(int id, TrangThaiBuoiKham trangThaiMoi, string? ghiChu = null);

        // Bác sĩ chủ động dời lịch
        bool DoiLichKham(int id, DateOnly ngayMoi, TimeOnly gioMoi, string lyDo);

        // Bệnh nhân xin dời lịch (trạng thái về Chờ xác nhận)
        bool BenhNhanYeuCauDoiLich(int id, DateOnly ngayMoi, TimeOnly gioMoi, string lyDo);

        // Lưu đánh giá sao và nhận xét
        bool LuuDanhGiaCuaBenhNhan(int id, int soSao, string nhanXet);
    }
}