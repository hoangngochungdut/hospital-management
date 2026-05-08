using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IBuoiKhamService
    {
        // ==================== READ ====================

        // Admin: lấy toàn bộ + filter
        Task<List<BuoiKham>> LayToanBoLichKhamAdminAsync(
            int? chuyenKhoaId = null,
            string? tenBacSi = null
        );

        List<BuoiKham> GetByBacSiId(int bacSiId);
        List<BuoiKham> GetByBenhNhanId(int benhNhanId);

        BuoiKham GetById(int id);

        Task<List<ChuyenKhoa>> LayTatCaChuyenKhoaAsync();


        // ==================== ĐẶT LỊCH ====================

        Task<object> LayBacSiVaPhongTheoKhoaAsync(int chuyenKhoaId);

        Task<List<string>> LayCacGioKhamTrongAsync(
            int bacSiId,
            int phongKhamId,
            DateOnly ngay
        );

        Task<bool> DatLichKhamAsync(
            DatLichRequest request,
            int currentUserId,
            string role
        );


        // ==================== TRẠNG THÁI & DỜI LỊCH ====================

        //bool CapNhatTrangThai(
        //    int id,
        //    TrangThaiBuoiKham trangThaiMoi,
        //    string? ghiChu = null
        //);
        bool XulyCaKham(int id, TrangThaiBuoiKham trangThaiMoi, string? ghiChu = null, string? ketQuaKhamBenh = null);
        bool DoiLichKham(
            int id,
            DateOnly ngayMoi,
            TimeOnly gioMoi,
            string lyDo
        );

        bool BenhNhanYeuCauDoiLich(
            int id,
            DateOnly ngayMoi,
            TimeOnly gioMoi,
            string lyDo
        );

        bool LuuDanhGiaCuaBenhNhan(
            int id,
            int soSao,
            string nhanXet
        );
        bool DoiBacSi(int id,
            int bacSiMoiId,
            string lyDo);
        bool XoaLichKham(int id);
        void CapNhatThanhToan(int lichKhamId);
    }
}