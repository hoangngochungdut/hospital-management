using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq; // Bắt buộc phải có để dùng .Any()
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Implementations
{
    public class BuoiKhamService : IBuoiKhamService
    {
        private readonly IBuoiKhamRepository _buoiKhamRepo;
        private readonly AppDbContext _context;

        public BuoiKhamService(IBuoiKhamRepository buoiKhamRepo, AppDbContext context)
        {
            _buoiKhamRepo = buoiKhamRepo;
            _context = context;
        }

        // =====================================================================
        // 1. HÀM MỚI: LẤY DATA CHO DROPDOWN (CHỌN KHOA -> HIỆN BÁC SĨ & PHÒNG)
        // =====================================================================
        public async Task<object> LayBacSiVaPhongTheoKhoaAsync(int chuyenKhoaId)
        {
            // Lấy danh sách bác sĩ thuộc chuyên khoa
            var bacSis = await _context.BacSis
                .Where(b => b.ChuyenKhoaId == chuyenKhoaId)
                .Select(b => new {
                    id = b.Id,
                    ten = "BS. " + b.HoTen // Thêm chữ BS cho chuyên nghiệp
                })
                .ToListAsync();

            // Lấy danh sách phòng khám thuộc chuyên khoa
            var phongs = await _context.PhongKhams
                .Where(p => p.ChuyenKhoaId == chuyenKhoaId)
                .Select(p => new {
                    id = p.Id,
                    ten = "Phòng " + p.SoPhong + " (Tầng " + p.Tang + ") "  + p.LoaiPhong
                })
                .ToListAsync();

            // Trả về một cục object chứa cả 2 danh sách
            return new { BacSis = bacSis, Phongs = phongs };
        }

        // =====================================================================
        // 2. HÀM ĐẶT LỊCH (ĐÃ BỔ SUNG CHECK TRÙNG CỰC GẮT)
        // =====================================================================
        public async Task<bool> DatLichKhamAsync(DatLichRequest request, int currentUserId, string role)
        {
            // --- CHỐT CHẶN 1: KHÔNG CHO CHỌN NGÀY QUÁ KHỨ ---
            var homNay = DateOnly.FromDateTime(DateTime.Now);
            if (request.Ngay < homNay)
            {
                throw new Exception("Lỗi: Không thể đặt lịch cho ngày trong quá khứ!");
            }

            // --- CHỐT CHẶN 2: LẤY CÁC CA ĐÃ ĐẶT ĐỂ SO SÁNH ---
            var cacCaDaDat = await _buoiKhamRepo.GetCacCaDaDatAsync(request.Ngay, request.Gio);

            // Check trùng Bác sĩ (Bác sĩ không thể khám 2 người cùng 1 giờ)
            bool trungBacSi = cacCaDaDat.Any(b => b.BacSiId == request.BacSiId);
            if (trungBacSi)
            {
                throw new Exception("Lỗi: Bác sĩ này đã có lịch trực hoặc lịch khám vào khung giờ này.");
            }

            // Check trùng Phòng (1 Phòng không thể có 2 bác sĩ chui vào cùng lúc)
            bool trungPhong = cacCaDaDat.Any(b => b.PhongKhamId == request.PhongKhamId);
            if (trungPhong)
            {
                throw new Exception("Lỗi: Phòng khám này đã được sử dụng vào khung giờ này.");
            }

            // --- VƯỢT QUA HẾT CÁC ẢI THÌ CHẠY LOGIC PHÂN QUYỀN CỦA ÔNG ---
            var buoiKham = new BuoiKham
            {
                Ngay = request.Ngay,
                Gio = request.Gio,
                BacSiId = request.BacSiId,
                PhongKhamId = request.PhongKhamId,
                TrangThai = TrangThaiBuoiKham.ChuaXacNhan
            };

            if (role == "BenhNhan")
            {
                buoiKham.BenhNhanId = currentUserId;
            }
            else if (role == "Admin" || role == "LeTan")
            {
                if (!request.BenhNhanId.HasValue || request.BenhNhanId.Value <= 0)
                    throw new Exception("Vui lòng chọn bệnh nhân để đặt lịch hộ.");

                buoiKham.BenhNhanId = request.BenhNhanId.Value;
                buoiKham.TrangThai = TrangThaiBuoiKham.XacNhan;
            }
            else
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng này.");
            }

            return await _buoiKhamRepo.AddAsync(buoiKham);
        }
        // Hàm này cực kỳ quan trọng để đổ dữ liệu vào cái ô Dropdown đầu tiên (Chuyên khoa)
        public async Task<List<ChuyenKhoa>> LayTatCaChuyenKhoaAsync()
        {
            return await _context.ChuyenKhoas.OrderBy(x => x.TenKhoa).ToListAsync();
        }
        public async Task<bool> CapNhatTrangThaiAsync(int buoiKhamId, TrangThaiBuoiKham trangThaiMoi, int currentUserId, string role)
        {
            var buoiKham = await _buoiKhamRepo.GetByIdAsync(buoiKhamId);
            if (buoiKham == null)
                throw new Exception("Không tìm thấy buổi khám.");

            if (role == "BacSi")
            {
                if (buoiKham.BacSiId != currentUserId)
                    throw new UnauthorizedAccessException("Bạn không thể sửa lịch của bác sĩ khác.");
            }
            else if (role == "BenhNhan")
            {
                if (buoiKham.BenhNhanId != currentUserId || trangThaiMoi != TrangThaiBuoiKham.Huy)
                    throw new UnauthorizedAccessException("Bệnh nhân chỉ được phép tự hủy lịch của mình.");
            }
            else if (role != "Admin" && role != "LeTan")
            {
                throw new UnauthorizedAccessException("Tài khoản không hợp lệ.");
            }

            buoiKham.TrangThai = trangThaiMoi;
            return await _buoiKhamRepo.UpdateAsync(buoiKham);
        }

        //public List<BuoiKham> GetByBacSiId(int bacSiId)
        //{
        //    return _buoiKhamRepo.GetByBacSiId(bacSiId);
        //}
        public List<BuoiKham> GetByBacSiId(int bacSiId)
        {
            return _context.BuoiKhams
                .Include(b => b.BenhNhan) // Để lấy tên bệnh nhân
                .Include(b => b.PhongKham) // Để lấy tên phòng
                .Where(b => b.BacSiId == bacSiId)
                .OrderByDescending(b => b.Ngay) // Hiện ngày mới nhất lên đầu
                .ThenBy(b => b.Gio)            // Sắp xếp theo giờ khám
                .ToList();
        }
        public List<BuoiKham> GetByBenhNhanId(int benhNhanId)
        {
            return _context.BuoiKhams
                .Include(b => b.BacSi)      // Lấy tên Bác sĩ để hiển thị cho Bệnh nhân
                .Include(b => b.PhongKham)  // Lấy tên phòng
                .Where(b => b.BenhNhanId == benhNhanId)
                .OrderByDescending(b => b.Ngay)
                .ThenByDescending(b => b.Gio)
                .ToList();
        }
        public List<BuoiKham> GetAllLichKham()
        {
            return _buoiKhamRepo.GetAll();
        }

        public BuoiKham GetById(int id)
        {
            var lich = _buoiKhamRepo.GetById(id);
            if (lich == null)
                throw new Exception("Không tìm thấy lịch");
            return lich;
        }
        public bool XoaBuoiKham(int id)
        {
            var lich = _context.BuoiKhams.Find(id);
            if (lich != null)
            {
                _context.BuoiKhams.Remove(lich);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool CapNhatTrangThai(int id, TrangThaiBuoiKham trangThaiMoi)
        {
            var lich = _buoiKhamRepo.GetById(id);
            if (lich == null)
                return false;

            lich.TrangThai = trangThaiMoi;
            _context.SaveChanges();
            return true;
        }
        // Nhớ import cái namespace chứa Enum ở trên cùng của file nhé:
        // using QuanLyPhongKham.Models.Enums;
        public async Task<List<BenhNhan>> LayTatCaBenhNhanAsync()
        {
            // Nếu Bệnh nhân kế thừa Người dùng (có bảng cha con) thì thêm .Include(b => b.NguoiDung)
            // Nếu không thì chỉ cần _context.BenhNhans.ToListAsync() là đủ

            return await _context.BenhNhans
                // .Include(b => b.NguoiDung) // Bỏ comment dòng này nếu DB ông tách bảng NguoiDung
                .ToListAsync();
        }
        public async Task<List<string>> LayCacGioKhamTrongAsync(int bacSiId, int phongKhamId, DateOnly ngayKham)
        {
            // 1. Định nghĩa TẤT CẢ các ca khám hiển thị trên Web
            var tatCaCaKham = new List<string>
            {
             "07:00 - 08:00", "08:00 - 09:00", "09:00 - 10:00", "10:00 - 11:00",
             "13:00 - 14:00", "14:00 - 15:00", "15:00 - 16:00", "16:00 - 17:00"
             };

            // 2. Query DB: Lấy danh sách 'Giờ' ĐÃ ĐẶT (Kiểu TimeOnly)
            // - Ngay là DateOnly nên so sánh trực tiếp (không dùng .Date nữa)
            // - TrangThai dùng Enum TrangThaiBuoiKham.Huy
            var cacGioDaDat = await _context.BuoiKhams
                .Where(b => b.BacSiId == bacSiId
                         && b.PhongKhamId == phongKhamId
                         && b.Ngay == ngayKham
                         && b.TrangThai != TrangThaiBuoiKham.Huy)
                .Select(b => b.Gio) // Trả về List<TimeOnly>
                .ToListAsync();

            // 3. Biến TimeOnly (VD: 07:00) thành chuỗi "07:00 - 08:00" để so sánh
            var cacCaDaKín = cacGioDaDat.Select(g =>
                $"{g:HH\\:mm} - {g.AddHours(1):HH\\:mm}"
            ).ToList();

            // 4. Phép trừ tập hợp: Lấy (Tất cả) trừ đi (Đã kín) = (Còn trống)
            var cacCaConTrong = tatCaCaKham.Except(cacCaDaKín).ToList();

            return cacCaConTrong;
        }
    }
}