using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace QuanLyPhongKham.Services.Implementations
{
    public class LeTanService : ILeTanService
    {
        private readonly INguoiDungRepository _nguoiDungRepo;
        private readonly AppDbContext _context;

        public LeTanService(INguoiDungRepository nguoiDungRepo, AppDbContext context)
        {
            _nguoiDungRepo = nguoiDungRepo;
            _context = context;
        }
        //public async Task<IEnumerable<object>> SearchBenhNhanAsync(string term)
        //{
        //    if (string.IsNullOrWhiteSpace(term))
        //    {
        //        return new List<object>();
        //    }

        //    var lowerTerm = term.ToLower();

        //    // LƯU Ý: Nếu Bệnh Nhân của ông kế thừa Người Dùng giống như Bác Sĩ, 
        //    // ông gọi thẳng thuộc tính HoTen và Sdt như bên dưới.
        //    var benhNhans = await _context.BenhNhans
        //        .Where(b => b.HoTen.ToLower().Contains(lowerTerm) || b.Sdt.Contains(lowerTerm))
        //        .Select(b => new
        //        {
        //            id = b.Id,
        //            hoTen = b.HoTen,
        //            sdt = b.Sdt
        //        })
        //        .Take(10) // Giới hạn trả về 10 người cho nhẹ Web
        //        .ToListAsync();

        //    return benhNhans;
        //}
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