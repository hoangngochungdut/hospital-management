using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Services.Implementations
{
    public class BacSiService : IBacSiService
    {
        private readonly IBacSiRepository _bacSiRepo;
        private readonly ITaiKhoanRepository _taiKhoanRepo;
        private readonly IChuyenKhoaRepository _chuyenKhoaRepo;

        public BacSiService(
            IBacSiRepository bacSiRepo,
            ITaiKhoanRepository taiKhoanRepo,
            IChuyenKhoaRepository chuyenKhoaRepo)
        {
            _bacSiRepo = bacSiRepo;
            _taiKhoanRepo = taiKhoanRepo;
            _chuyenKhoaRepo = chuyenKhoaRepo;
        }

        public ICollection<BacSi> GetAll()
        {
            return _bacSiRepo.GetAll();
        }

        public XemHoSoBacSiResponse? GetHoSo(int nguoiDungId)
        {
            return _bacSiRepo.GetHoSo(nguoiDungId);
        }

        public (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBacSiRequest request)
        {
            try
            {
                var bacSi = _bacSiRepo.GetById(nguoiDungId);

                if (bacSi == null)
                    return (false, "Không tìm thấy bác sĩ");

                bacSi.HoTen = request.HoTen;
                bacSi.GioiTinh = request.GioiTinh;
                bacSi.DiaChi = request.DiaChi;
                bacSi.Sdt = request.SoDienThoai;
                bacSi.ChuyenKhoaId = request.ChuyenKhoaId;

                _bacSiRepo.Update(bacSi);

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

        public BacSi? GetById(int id)
        {
            return _bacSiRepo.GetById(id);
        }

        // ✅ CHỈ GIỮ CHUYÊN KHOA
        public ICollection<ChuyenKhoa> GetDanhSachChuyenKhoa()
        {
            return _chuyenKhoaRepo.GetAll();
        }
    }
}