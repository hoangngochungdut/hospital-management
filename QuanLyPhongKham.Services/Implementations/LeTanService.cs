using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Implementations;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Services.Implementations
{
    public class LeTanService : ILeTanService
    {
        private readonly INguoiDungRepository _nguoiDungRepo;
        private readonly ITaiKhoanRepository _taiKhoanRepo;
        private readonly ILeTanRepository _letanRepository;

        public LeTanService(INguoiDungRepository nguoiDungRepo, ITaiKhoanRepository taiKhoanRepo, ILeTanRepository letanRepository)
        {
            _nguoiDungRepo = nguoiDungRepo;
            _taiKhoanRepo = taiKhoanRepo;
            _letanRepository = letanRepository;
        }

        public XemHoSoLeTanResponse? GetHoSo(int nguoiDungId)
        {
            var nguoiDung = _nguoiDungRepo.GetById(nguoiDungId);
            if (nguoiDung == null) return null;

            return new XemHoSoLeTanResponse
            {
                NguoiDungId = nguoiDung.Id,
                HoTen = nguoiDung.HoTen ?? string.Empty,
                GioiTinh = nguoiDung.GioiTinh,
                DiaChi = nguoiDung.DiaChi,
                SoDienThoai = nguoiDung.Sdt
            };
        }

        public (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoLeTanRequest request)
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

        public ICollection<LeTan> GetAll()
        {
            return _letanRepository.GetAll();
        }

        public LeTan? GetByIdWithTaiKhoan(int id)
        {
            return _letanRepository.GetByIdWithTaiKhoan(id);
        }

        public void Add(AddLeTanDto model)
        {
            // 1. Tạo tài khoản trước (Đã đổi thành MatKhauHash cho chuẩn logic của ông)
            var taiKhoanMoi = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap,
                MatKhauHash = model.MatKhau,
                VaiTro = "LT"
            };

            // 2. Tạo Lễ tân và nhét cái Tài khoản vào
            var leTanMoi = new LeTan
            {
                HoTen = model.HoTen,
                GioiTinh = model.GioiTinh,
                Sdt = model.SoDienThoai,
                Email = model.Email,
                DiaChi = model.DiaChi,
                TaiKhoan = taiKhoanMoi
            };

            _letanRepository.Add(leTanMoi);
        }

        public void Update(int id, CapNhatHoSoLeTanRequest model)
        {
            var leTan = _letanRepository.GetById(id);
            if (leTan != null)
            {
                leTan.HoTen = model.HoTen;
                leTan.GioiTinh = model.GioiTinh;
                leTan.Sdt = model.SoDienThoai;
                leTan.Email = model.Email;
                leTan.DiaChi = model.DiaChi;

                _letanRepository.Update(leTan);
            }
        }

        public void Delete(int id)
        {
            var leTan = _letanRepository.GetById(id);
            if (leTan != null)
            {
                _letanRepository.Delete(leTan);
            }
        }
    }
}