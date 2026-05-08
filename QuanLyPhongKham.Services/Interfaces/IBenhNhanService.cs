using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IBenhNhanService
    {
        // =========================
        // HỒ SƠ
        // =========================
        XemHoSoBenhNhanResponse? GetHoSo(int nguoiDungId);
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBenhNhanRequest request);

        // =========================
        // MẬT KHẨU
        // =========================
        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);

        // =========================
        // LẤY DANH SÁCH
        // =========================
        Task<IEnumerable<BenhNhan>> GetAllAsync();
        ICollection<BenhNhan> GetAll();

        // =========================
        // TIỂU SỬ BỆNH NHÂN
        // =========================
        TieuSuBenhNhan GetTieuSu(int benhNhanId);

        // =========================
        // CRUD
        // =========================
        void Add(AddBenhNhanDto entity);
        BenhNhan? GetById(int id);
        BenhNhan? GetByIdWithTaiKhoan(int id);
        void Update(int id, CapNhatHoSoBenhNhanRequest request);
        void Delete(int id);
    }
}