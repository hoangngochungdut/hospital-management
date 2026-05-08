using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Implementations;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
            return _bacSiRepo
                .GetAllWithTaiKhoan()
                .Where(bs => bs.TaiKhoan != null)
                .ToList();
        }

        public XemHoSoBacSiResponse? GetHoSo(int nguoiDungId)
        {
            return _bacSiRepo.GetHoSo(nguoiDungId);
        }

        // ✅ HÀM ĐỒNG BỘ: Không dùng async/await để khớp Interface nhóm
        public (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBacSiRequest request)
        {
            try
            {
                // Gọi đồng bộ, bacSi sẽ nhận đúng đối tượng chứ không phải Task
                var bacSi = _bacSiRepo.GetById(nguoiDungId);

                if (bacSi == null)
                    return (false, "Không tìm thấy bác sĩ");

                // Giờ thì chấm HoTen thoải mái không lo lỗi
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

        // ✅ ĐÃ SỬA: Trả về BacSi thường, không dùng Task nữa
        public BacSi GetById(int id)
        {
            return _bacSiRepo.GetById(id);
        }

        public ICollection<ChuyenKhoa> GetDanhSachChuyenKhoa()
        {
            return _chuyenKhoaRepo.GetAll();
        }

        public async Task<IEnumerable<BacSi>> GetByChuyenKhoaIdAsync(int chuyenKhoaId)
        {
            return await _bacSiRepo.GetByChuyenKhoaIdAsync(chuyenKhoaId);
        }
        public List<BacSiDanhGiaViewModel> LayDanhSachBacSiVaDanhGia()
        {
            // 1. Gọi Repo lấy data thô (đã được Include đầy đủ bảng)
            var danhSachDataTho = _bacSiRepo.GetDanhSachBacSiKemDanhGia();

            // 2. Chuyển đổi (Map) sang ViewModel và tính toán logic
            var ketQua = danhSachDataTho.Select(bs => new BacSiDanhGiaViewModel
            {
                BacSiId = bs.Id,
                TenBacSi = bs.HoTen,
                TenChuyenKhoa = bs.ChuyenKhoa?.TenKhoa ?? "Đang cập nhật",

                // Tính tổng số lượt đánh giá
                TongSoDanhGia = bs.BuoiKhams.Count(bk => bk.DiemDanhGia != null),

                // Tính trung bình sao
                DiemTrungBinh = bs.BuoiKhams.Any(bk => bk.DiemDanhGia != null)
                    ? Math.Round(bs.BuoiKhams.Where(bk => bk.DiemDanhGia != null).Average(bk => bk.DiemDanhGia.Value), 1)
                    : 0,

                // Lấy 5 nhận xét mới nhất
                DanhSachNhanXet = bs.BuoiKhams
                    .Where(bk => bk.DiemDanhGia != null)
                    .OrderByDescending(bk => bk.Ngay)
                    .Take(5)
                    .Select(bk => new ChiTietDanhGia
                    {
                        SoSao = bk.DiemDanhGia.Value,
                        NhanXet = bk.NhanXetCuaBenhNhan,
                        TenBenhNhan = bk.BenhNhan?.HoTen ?? "Bệnh nhân ẩn danh",
                        NgayKham = bk.Ngay
                    }).ToList()
            }).ToList();

            return ketQua;
        }

        public BacSi? GetByIdWithTaiKhoan(int id)
        {
            return _bacSiRepo.GetByIdWithTaiKhoan(id);
        }

        public void Add(AddBacSiDto entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = entity.TenDangNhap,
                MatKhauHash = entity.MatKhau,
                VaiTro = "BS"
            };

            var bacSi = new BacSi
            {
                HoTen = entity.HoTen,
                Sdt = entity.SoDienThoai,
                Email = entity.Email,
                ChuyenKhoaId = entity.ChuyenKhoaId,
                DiaChi = entity.DiaChi,
                GioiTinh = entity.GioiTinh,
                TaiKhoan = taiKhoan
            };

            _bacSiRepo.Add(bacSi);
        }

        public void Update(int id, CapNhatHoSoBacSiRequest bacsi)
        {
            var existing = _bacSiRepo.GetById(id);

            if (existing == null)
                throw new Exception("Không tìm thấy bác sĩ");

            existing.HoTen = bacsi.HoTen;
            existing.Sdt = bacsi.SoDienThoai;
            existing.Email = bacsi.Email;
            existing.DiaChi = bacsi.DiaChi;
            existing.GioiTinh = bacsi.GioiTinh;
            existing.ChuyenKhoaId = bacsi.ChuyenKhoaId;

            _bacSiRepo.Update(existing);
        }

        public void Delete(int id)
        {
            var bacsi = _bacSiRepo.GetByIdWithTaiKhoan(id);

            if (bacsi == null)
            {
                throw new Exception("Bệnh nhân không tồn tại");
            }

            if (bacsi.TaiKhoan != null)
            {
                _taiKhoanRepo.Delete(bacsi.TaiKhoan);
            }
        }
    }
}