using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Services.Implementations
{
    public class BenhNhanService : IBenhNhanService
    {
        private readonly IBenhNhanRepository _benhNhanRepository;
        private readonly INguoiDungRepository _nguoiDungRepository;
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly IBuoiKhamRepository _buoiKhamRepository;
        private readonly IKetQuaKhamRepository _ketQuaRepository;
        private readonly ITieuSuBenhAnRepository _tieuSuRepository;

        public BenhNhanService(
            IBenhNhanRepository benhNhanRepository,
            INguoiDungRepository nguoiDungRepository,
            ITaiKhoanRepository taiKhoanRepository,
            IBuoiKhamRepository buoiKhamRepository,
            IKetQuaKhamRepository ketQuaRepository,
            ITieuSuBenhAnRepository tieuSuRepository)
        {
            _benhNhanRepository = benhNhanRepository;
            _nguoiDungRepository = nguoiDungRepository;
            _taiKhoanRepository = taiKhoanRepository;
            _buoiKhamRepository = buoiKhamRepository;
            _ketQuaRepository = ketQuaRepository;
            _tieuSuRepository = tieuSuRepository;
        }

        // =========================
        // HỒ SƠ
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

        // =========================
        // DANH SÁCH
        // =========================
        public async Task<IEnumerable<BenhNhan>> GetAllAsync()
        {
            return await _benhNhanRepository.GetAllAsync();
        }

        public ICollection<BenhNhan> GetAll()
        {
            return _benhNhanRepository
                .GetAllWithTaiKhoan()
                .Where(x => x.TaiKhoan != null)
                .ToList();
        }

        // =========================
        // MẬT KHẨU
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

        // =========================
        // TIỂU SỬ
        // =========================
        public TieuSuBenhNhan GetTieuSu(int benhNhanId)
        {
            var hoSo = GetHoSo(benhNhanId) ?? new XemHoSoBenhNhanResponse();

            var buoiKhams = _buoiKhamRepository.GetByBenhNhanId(benhNhanId)
                             ?? new List<BuoiKham>();

            var tieuSu = _tieuSuRepository.GetByBenhNhanId(benhNhanId);

            var lichSu = new List<LichSuKhamDTO>();

            foreach (var bk in buoiKhams)
            {
                var ketQua = _ketQuaRepository.GetByBuoiKhamId(bk.Id);

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

        // =========================
        // CRUD
        // =========================
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

            var existing = _taiKhoanRepository.GetByUsername(entity.TenDangNhap);
            if (existing != null)
                throw new Exception("Tên đăng nhập đã tồn tại");

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

            var bn = _benhNhanRepository.GetById(id);
            if (bn == null)
                throw new Exception("Không tìm thấy bệnh nhân");

            bn.HoTen = request.HoTen;
            bn.GioiTinh = request.GioiTinh;
            bn.DiaChi = request.DiaChi;
            bn.Sdt = request.SoDienThoai;
            bn.Email = request.Email;

            _benhNhanRepository.Update(bn);
        }

        public void Delete(int id)
        {
            var bn = _benhNhanRepository.GetByIdWithTaiKhoan(id);
            if (bn == null)
                throw new Exception("Bệnh nhân không tồn tại");

            if (bn.TaiKhoan != null)
                _taiKhoanRepository.Delete(bn.TaiKhoan);
        }
    }
}