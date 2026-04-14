using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Services.Implementations
{
    public class BacSiService : IBacSiService
    {
        private readonly INguoiDungRepository _nguoiDungRepo;
        private readonly IBacSiRepository _bacSiRepo;
        private readonly AppDbContext _context;

        public BacSiService(INguoiDungRepository nguoiDungRepo, IBacSiRepository bacSiRepo, AppDbContext context)
        {
            _nguoiDungRepo = nguoiDungRepo;
            _bacSiRepo = bacSiRepo;
            _context = context;
        }

        public XemHoSoBacSiResponse? GetHoSo(int nguoiDungId)
        {
            var nguoiDung = _nguoiDungRepo.GetById(nguoiDungId);
            if (nguoiDung == null) return null;

            var bacSi = _bacSiRepo.GetByNguoiDungId(nguoiDungId);
            if (bacSi == null) return null;

            var chuyenKhoa = _context.ChuyenKhoas.FirstOrDefault(x => x.Id == bacSi.ChuyenKhoaId);
            var phongKham = _context.PhongKhams.FirstOrDefault(x => x.Id == bacSi.PhongLamViecId);

            return new XemHoSoBacSiResponse
            {
                HoTen = nguoiDung.HoTen ?? string.Empty,
                GioiTinh = nguoiDung.GioiTinh,
                DiaChi = nguoiDung.DiaChi,
                SoDienThoai = nguoiDung.Sdt,
                TenChuyenKhoa = chuyenKhoa?.TenKhoa,
                TenPhong = phongKham?.SoPhong.ToString()
            };
        }

        public (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBacSiRequest request)
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

                var bacSi = _bacSiRepo.GetByNguoiDungId(nguoiDungId);
                if (bacSi != null)
                {
                    bacSi.ChuyenKhoaId = request.ChuyenKhoaId;
                    bacSi.PhongLamViecId = request.PhongLamViecId;
                    _bacSiRepo.Update(bacSi);
                }

                return (true, "Cập nhật thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request)
        {
            try
            {
                var taiKhoan = _context.TaiKhoans.FirstOrDefault(x => x.NguoiDungId == nguoiDungId);
                if (taiKhoan == null)
                    return (false, "Không tìm thấy tài khoản");

                if (taiKhoan.MatKhauHash != request.MatKhauCu)
                    return (false, "Mật khẩu cũ không đúng");

                taiKhoan.MatKhauHash = request.MatKhauMoi;
                await _context.SaveChangesAsync();
                return (true, "Đổi mật khẩu thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
    }
}