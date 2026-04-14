using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Data;  // 👈 THÊM DÒNG NÀY (cho AppDbContext)
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Implementations
{
    public  class BenhNhanService : IBenhNhanService
    {
        private readonly INguoiDungRepository _nguoiDungRepo;
        private readonly AppDbContext _context;  // 👈 THÊM DÒNG NÀY

        public BenhNhanService(INguoiDungRepository nguoiDungRepo, AppDbContext context)
        {
            _nguoiDungRepo = nguoiDungRepo;
            _context = context;        }

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
        public async Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request)
        {
            try
            {
                // Lấy thông tin tài khoản
                var taiKhoan = _context.TaiKhoans.FirstOrDefault(tk => tk.NguoiDungId == nguoiDungId);
                if (taiKhoan == null)
                    return (false, "Không tìm thấy tài khoản");

                // Kiểm tra mật khẩu cũ (nếu dùng plain text)
                if (taiKhoan.MatKhauHash != request.MatKhauCu)
                    return (false, "Mật khẩu cũ không đúng");

                // Cập nhật mật khẩu mới
                taiKhoan.MatKhauHash = request.MatKhauMoi;
                _context.TaiKhoans.Update(taiKhoan);
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
