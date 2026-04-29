using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Implementations
{
    public class LeTanService : ILeTanService
    {
        private readonly INguoiDungRepository _nguoiDungRepo;
        private readonly ITaiKhoanRepository _taiKhoanRepo;
        private readonly AppDbContext _context;

        public LeTanService(
            INguoiDungRepository nguoiDungRepo,
            ITaiKhoanRepository taiKhoanRepo,
            AppDbContext context)
        {
            _nguoiDungRepo = nguoiDungRepo;
            _taiKhoanRepo = taiKhoanRepo;
            _context = context;
        }

        // ==========================================
        // 1. QUẢN LÝ THANH TOÁN (LỄ TÂN)
        // ==========================================

        public async Task<IEnumerable<BuoiKham>> GetDanhSachChoThanhToanAsync()
        {
            return await _context.BuoiKhams
                .Include(b => b.BenhNhan)
                .Include(b => b.BacSi)
                .Where(b => b.TrangThai != TrangThaiBuoiKham.Huy && b.HoaDon == null)
                .OrderByDescending(b => b.Id)
                .ToListAsync();
        }

        public async Task<bool> XacNhanThanhToanAsync(int buoiKhamId, decimal tongTien)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var buoiKham = await _context.BuoiKhams.FindAsync(buoiKhamId);
                if (buoiKham == null) return false;

                var hoaDon = new HoaDon
                {
                    BuoiKhamId = buoiKhamId,
                    TongTien = tongTien,
                    DaThanhToan = true,
                    NgayThanhToan = DateTime.Now
                };

                _context.HoaDons.Add(hoaDon);
                var result = await _context.SaveChangesAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // ==========================================
        // 2. QUẢN LÝ BỆNH NHÂN (LỄ TÂN)
        // ==========================================

        public async Task<IEnumerable<BenhNhan>> TimKiemBenhNhanAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return await _context.BenhNhans.ToListAsync();

            return await _context.BenhNhans
                .Where(b => b.HoTen.ToLower().Contains(keyword.ToLower()) || b.Sdt.Contains(keyword))
                .ToListAsync();
        }

        public async Task<BenhNhan?> GetChiTietBenhNhanAsync(int id)
        {
            return await _context.BenhNhans
                .Include(b => b.BuoiKhams)
                    .ThenInclude(bk => bk.BacSi)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // ==========================================
        // 3. QUẢN LÝ HỒ SƠ CÁ NHÂN (LỄ TÂN)
        // ==========================================

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
                if (nguoiDung == null) return (false, "Không tìm thấy thông tin người dùng");

                nguoiDung.HoTen = request.HoTen;
                nguoiDung.GioiTinh = request.GioiTinh;
                nguoiDung.DiaChi = request.DiaChi;
                nguoiDung.Sdt = request.SoDienThoai;

                _nguoiDungRepo.Update(nguoiDung);
                return (true, "Cập nhật hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request)
        {
            try
            {
                var taiKhoan = _taiKhoanRepo.GetByNguoiDungId(nguoiDungId);
                if (taiKhoan == null) return (false, "Không tìm thấy thông tin tài khoản");

                if (taiKhoan.MatKhauHash != request.MatKhauCu)
                    return (false, "Mật khẩu cũ không chính xác");

                taiKhoan.MatKhauHash = request.MatKhauMoi;
                _taiKhoanRepo.Update(taiKhoan);

                return await Task.FromResult((true, "Đổi mật khẩu thành công"));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // ==========================================
        // 4. ADMIN QUẢN LÝ LỄ TÂN (FIXED)
        // ==========================================

        public async Task<IEnumerable<LeTan>> GetAllLeTanAsync()
        {
            var query = from lt in _context.LeTans
                        join tk in _context.TaiKhoans on lt.Id equals tk.NguoiDungId into joined
                        from subTk in joined.DefaultIfEmpty()
                        select new { LeTan = lt, TaiKhoan = subTk };

            var results = await query.AsNoTracking().ToListAsync();

            foreach (var item in results)
            {
                item.LeTan.TaiKhoan = item.TaiKhoan;
            }

            return results.Select(x => x.LeTan);
        }

        public async Task<LeTan?> GetByIdAsync(int id)
        {
            return await _context.LeTans
                .Include(lt => lt.TaiKhoan)
                .FirstOrDefaultAsync(lt => lt.Id == id);
        }

        public async Task<(bool Success, string Message)> CreateLeTanAsync(LeTan leTan, string tenDangNhap, string matKhau)
        {
            if (await _context.TaiKhoans.AnyAsync(tk => tk.TenDangNhap == tenDangNhap))
                return (false, "Tên đăng nhập đã tồn tại!");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                leTan.Id = 0;
                _context.LeTans.Add(leTan);
                await _context.SaveChangesAsync();

                var taiKhoan = new TaiKhoan
                {
                    NguoiDungId = leTan.Id,
                    TenDangNhap = tenDangNhap,
                    MatKhauHash = matKhau,
                    // SỬA TẠI ĐÂY: Đổi "Receptionist" thành "LT" để khớp với AccountController
                    VaiTro = "LT"
                };

                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "Thêm lễ tân thành công!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Lỗi hệ thống: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        public async Task<(bool Success, string Message)> UpdateLeTanAsync(LeTan leTan)
        {
            try
            {
                var existing = await _context.LeTans.FindAsync(leTan.Id);
                if (existing == null) return (false, "Không tìm thấy nhân viên");

                existing.HoTen = leTan.HoTen;
                existing.Sdt = leTan.Sdt;
                existing.GioiTinh = leTan.GioiTinh;
                existing.DiaChi = leTan.DiaChi;

                await _context.SaveChangesAsync();
                return (true, "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return (false, "Lỗi cập nhật: " + ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> DeleteLeTanAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(tk => tk.NguoiDungId == id);
                if (taiKhoan != null) _context.TaiKhoans.Remove(taiKhoan);

                var leTan = await _context.LeTans.FindAsync(id);
                if (leTan != null) _context.LeTans.Remove(leTan);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (true, "Xóa lễ tân thành công");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Lỗi khi xóa: " + ex.Message);
            }
        }

        public async Task<IEnumerable<LeTan>> SearchLeTanAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return await GetAllLeTanAsync();

            var query = from lt in _context.LeTans
                        join tk in _context.TaiKhoans on lt.Id equals tk.NguoiDungId into joined
                        from subTk in joined.DefaultIfEmpty()
                        where lt.HoTen.ToLower().Contains(keyword.ToLower()) || lt.Sdt.Contains(keyword)
                        select new { LeTan = lt, TaiKhoan = subTk };

            var results = await query.AsNoTracking().ToListAsync();

            foreach (var item in results)
            {
                item.LeTan.TaiKhoan = item.TaiKhoan;
            }

            return results.Select(x => x.LeTan);
        }
    }
}