using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Services.Implementations
{
    public class BenhNhanService : IBenhNhanService
    {
        private readonly INguoiDungRepository _nguoiDungRepo;
        private readonly ITaiKhoanRepository _taiKhoanRepo;

        public BenhNhanService(
            INguoiDungRepository nguoiDungRepo,
            ITaiKhoanRepository taiKhoanRepo)
        {
            _nguoiDungRepo = nguoiDungRepo;
            _taiKhoanRepo = taiKhoanRepo;
        }

        // =========================
        // LẤY HỒ SƠ
        // =========================
        public XemHoSoBenhNhanResponse? GetHoSo(int nguoiDungId)
        {
            var nguoiDung = _nguoiDungRepo.GetById(nguoiDungId);
            if (nguoiDung == null) return null;

            return new XemHoSoBenhNhanResponse
            {
                NguoiDungId = nguoiDung.Id,
                HoTen = nguoiDung.HoTen ?? string.Empty,
                GioiTinh = nguoiDung.GioiTinh,
                DiaChi = nguoiDung.DiaChi,
                SoDienThoai = nguoiDung.Sdt
            };
        }

        // =========================
        // CẬP NHẬT HỒ SƠ
        // =========================
        public (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBenhNhanRequest request)
        {
            try
            {
                var nguoiDung = _nguoiDungRepo.GetById(nguoiDungId);
                if (nguoiDung == null)
                    return (false, "Không tìm thấy người dùng");

                nguoiDung.HoTen = request.HoTen;
                nguoiDung.GioiTinh = request.GioiTinh;
                nguoiDung.DiaChi = request.DiaChi;
                nguoiDung.Sdt = request.SoDienThoai;

                _nguoiDungRepo.Update(nguoiDung);

                return (true, "Cập nhật hồ sơ thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // =========================
        // ĐỔI MẬT KHẨU
        // =========================
        public async Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request)
        {
            try
            {
                var taiKhoan = _taiKhoanRepo.GetByNguoiDungId(nguoiDungId);
                if (taiKhoan == null)
                    return (false, "Không tìm thấy tài khoản");

                if (taiKhoan.MatKhauHash != request.MatKhauCu)
                    return (false, "Mật khẩu cũ không đúng");

                taiKhoan.MatKhauHash = request.MatKhauMoi;

                _taiKhoanRepo.Update(taiKhoan);

                return (true, "Đổi mật khẩu thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
    }
}