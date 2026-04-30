using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Implementations;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Services.Implementations
{
    public class BenhNhanService : IBenhNhanService
    {
        private readonly IBenhNhanRepository _benhNhanRepository;
        private readonly INguoiDungRepository _nguoiDungRepository;
        private readonly ITaiKhoanRepository _taiKhoanRepository;

        public BenhNhanService(
            IBenhNhanRepository benhNhanRepository,
            INguoiDungRepository nguoiDungRepo,
            ITaiKhoanRepository taiKhoanRepo,
            IBenhNhanRepository benhNhanRepo)
        {
            _benhNhanRepository = benhNhanRepository;
            _nguoiDungRepository = nguoiDungRepo;
            _taiKhoanRepository = taiKhoanRepo;
        }

        // =========================
        // LẤY HỒ SƠ
        // =========================
        public XemHoSoBenhNhanResponse? GetHoSo(int nguoiDungId)
        {
            var nguoiDung = _nguoiDungRepository.GetById(nguoiDungId);
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
                var nguoiDung = _nguoiDungRepository.GetById(nguoiDungId);
                if (nguoiDung == null)
                    return (false, "Không tìm thấy người dùng");

                nguoiDung.HoTen = request.HoTen;
                nguoiDung.GioiTinh = request.GioiTinh;
                nguoiDung.DiaChi = request.DiaChi;
                nguoiDung.Sdt = request.SoDienThoai;

                _nguoiDungRepository.Update(nguoiDung);

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
            return await _benhNhanRepository.GetAllAsync();
        }

        // =========================
        // ĐỔI MẬT KHẨU
        // =========================
        public async Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request)
        {
            try
            {
                var taiKhoan = _taiKhoanRepository.GetByNguoiDungId(nguoiDungId);
                if (taiKhoan == null)
                    return (false, "Không tìm thấy tài khoản");

                if (taiKhoan.MatKhauHash != request.MatKhauCu)
                    return (false, "Mật khẩu cũ không đúng");

                taiKhoan.MatKhauHash = request.MatKhauMoi;

                _taiKhoanRepository.Update(taiKhoan);

                return (true, "Đổi mật khẩu thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public ICollection<BenhNhan> GetAll()
        {
             return _benhNhanRepository
                .GetAllWithTaiKhoan()
                .Where(bs => bs.TaiKhoan != null)
                .ToList();
        }

        public BenhNhan? GetById(int id)
        {
            return _benhNhanRepository.GetById(id);
        }

        public BenhNhan? GetByIdWithTaiKhoan(int id)
        {
            return _benhNhanRepository.GetByIdWithTaiKhoan(id);
        }



        public void Add(AddBenhNhanDto entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // kiểm tra username đã tồn tại chưa
            var existingAccount = _taiKhoanRepository.GetByUsername(entity.TenDangNhap);

            if (existingAccount != null)
            {
                throw new Exception("Tên đăng nhập đã tồn tại");
            }

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = entity.TenDangNhap,
                MatKhauHash = entity.MatKhau,
                VaiTro = "BN"
            };

            var benhNhan = new BenhNhan
            {
                HoTen = entity.HoTen,
                GioiTinh = entity.GioiTinh,
                Sdt = entity.SoDienThoai,
                Email = entity.Email,
                DiaChi = entity.DiaChi,
                TaiKhoan = taiKhoan
            };

            _benhNhanRepository.Add(benhNhan);
        }

        public void Update(int id, CapNhatHoSoBenhNhanRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var benhnhan = _benhNhanRepository.GetById(id);

            if (benhnhan == null)
                throw new Exception("Không tìm thấy bệnh nhân");

            benhnhan.HoTen = request.HoTen;
            benhnhan.GioiTinh = request.GioiTinh;
            benhnhan.DiaChi = request.DiaChi;
            benhnhan.Sdt = request.SoDienThoai;
            benhnhan.Email = request.Email;

            _benhNhanRepository.Update(benhnhan);
        }

        public void Delete(int id)
        {
            var benhNhan = _benhNhanRepository.GetByIdWithTaiKhoan(id);

            if (benhNhan == null)
            {
                throw new Exception("Bệnh nhân không tồn tại");
            }

            if (benhNhan.TaiKhoan != null)
            {
                _taiKhoanRepository.Delete(benhNhan.TaiKhoan);
            }
        }
    }
}