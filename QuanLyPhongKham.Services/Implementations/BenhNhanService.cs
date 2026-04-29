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
        private readonly IBenhNhanRepository _benhNhanRepo;
        private readonly IBuoiKhamRepository _buoiKhamRepo;
        private readonly IKetQuaKhamRepository _ketQuaRepo;
        private readonly ITieuSuBenhAnRepository _tieuSuRepo;

        public BenhNhanService(
            INguoiDungRepository nguoiDungRepo,
            ITaiKhoanRepository taiKhoanRepo,
            IBenhNhanRepository benhNhanRepo,
            IBuoiKhamRepository buoiKhamRepo,
            IKetQuaKhamRepository ketQuaRepo,
            ITieuSuBenhAnRepository tieuSuRepo)
        {
            _nguoiDungRepo = nguoiDungRepo;
            _taiKhoanRepo = taiKhoanRepo;
            _benhNhanRepo = benhNhanRepo;
            _buoiKhamRepo = buoiKhamRepo;   
            _ketQuaRepo = ketQuaRepo;
            _tieuSuRepo = tieuSuRepo;
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
        public async Task<IEnumerable<BenhNhan>> GetAllAsync()
        {
            // Gọi thằng Repo lên làm việc
            return await _benhNhanRepo.GetAllAsync();
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
        // TSBN
        public TieuSuBenhNhan GetTieuSu(int benhNhanId)
        {
            var hoSo = GetHoSo(benhNhanId) ?? new XemHoSoBenhNhanResponse();

            var buoiKhams = _buoiKhamRepo.GetByBenhNhanId(benhNhanId)
                             ?? new List<BuoiKham>();

            var tieuSu = _tieuSuRepo.GetByBenhNhanId(benhNhanId);

            var lichSu = new List<LichSuKhamDTO>();

            foreach (var bk in buoiKhams)
            {
                var ketQua = _ketQuaRepo.GetByBuoiKhamId(bk.Id);

                lichSu.Add(new LichSuKhamDTO
                {
                    NgayKham = bk.Ngay.ToDateTime(TimeOnly.MinValue),
                    KetQua = ketQua?.KetQua ?? "Chưa có kết quả",
                    GhiChu = bk.GhiChuKetQua ?? ""
                });
            }

            return new TieuSuBenhNhan
            {
                ThongTin = hoSo,
                TienSuBenh = tieuSu?.MoTa ?? "Chưa có",
                LichSuKham = lichSu
            };
        
    }
    }
}