using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;        
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Implementations;// new
using QuanLyPhongKham.Services.Interfaces;  // new

namespace QuanLyPhongKham.Web.Controllers
{
    public class BenhNhanDashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBacSiService _bacSiService;
        private readonly IBenhNhanService _benhNhanService;  // 👈 THÊM DÒNG NÀY
        //public BenhNhanDashboardController(AppDbContext context, IBenhNhanService benhNhanService)
        //{
        //    _context = context;
        //    _benhNhanService = benhNhanService;  // 👈 THÊM DÒNG NÀY
        //}
        public BenhNhanDashboardController(AppDbContext context, IBacSiService bacSiService, IBenhNhanService benhNhanService)
        {
            _context = context;
            _bacSiService = bacSiService;
            _benhNhanService = benhNhanService;  // 👈 THÊM DÒNG NÀY
        }

        public IActionResult BenhNhanDashboard() 
        {
            //ViewBag.DsBacSi = _context.BacSis.ToList();
            ViewBag.DsBacSi = _bacSiService.GetAll();
            ViewBag.DsPhongKham = _context.PhongKhams.ToList();
            return View(); 
        }

        [HttpGet]
        public IActionResult LichKham()
        {
            // XÓA cái .Include(b => b.NguoiDung) đi, chỉ cần lấy ToList() là đủ
            ViewBag.DsBacSi = _context.BacSis.ToList();
            ViewBag.DsBenhNhan = _context.BenhNhans.ToList();
            ViewBag.DsPhongKham = _context.PhongKhams.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult DatLich(DateOnly Ngay, TimeOnly Gio, int BacSiId, int PhongKhamId)
        {
            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioBatDau = Gio;
            var gioKetThuc = Gio.AddMinutes(30);

            bool biTrung = _context.BuoiKhams.Any(x =>
                x.BacSiId == BacSiId &&
                x.Ngay == Ngay &&
                x.TrangThai != TrangThaiBuoiKham.Huy &&
                (
                    // overlap time
                    x.Gio < gioKetThuc &&
                    x.Gio.AddMinutes(30) > gioBatDau
                )
            );

            // 🔥 CHECK TRÙNG LỊCH
            //bool biTrung = _context.BuoiKhams.Any(x =>
            //    x.Ngay == Ngay &&
            //    x.Gio == Gio &&
            //    (
            //        x.BacSiId == BacSiId ||        // trùng bác sĩ
            //        x.PhongKhamId == PhongKhamId || // trùng phòng
            //        x.BenhNhanId == currentBenhNhanId // trùng bệnh nhân
            //    )
            //    && x.TrangThai != TrangThaiBuoiKham.Huy // bỏ qua lịch đã hủy
            //);

            if (biTrung)
            {
                TempData["ThongBao"] = "❌ Lịch bị trùng! Vui lòng chọn thời gian khác.";
                return RedirectToAction("LichKham");
            }

            // ✅ Nếu không trùng thì tạo mới
            var lichMoi = new BuoiKham
            {
                Ngay = Ngay,
                Gio = Gio,
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId,
                BenhNhanId = currentBenhNhanId.Value,
                TrangThai = TrangThaiBuoiKham.ChuaXacNhan
            };

            _context.BuoiKhams.Add(lichMoi);
            _context.SaveChanges();

            TempData["ThongBao"] = "✅ Đặt lịch thành công!";
            return RedirectToAction("LichKham");
        }

        // GET: Thông tin cá nhân (chế độ xem)
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            // Lấy UserId từ Session
            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem thông tin";
                return RedirectToAction("Login", "Account");
            }

            var benhNhan = _context.BenhNhans
                .FirstOrDefault(b => b.Id == currentBenhNhanId.Value);

            if (benhNhan == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin bệnh nhân";
                return RedirectToAction("BenhNhanDashboard");
            }

            var hoSo = _benhNhanService.GetHoSo(benhNhan.Id);

            if (hoSo == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("BenhNhanDashboard");
            }

            return View(hoSo);
        }
        // GET: Xem hồ sơ
        [HttpGet]
        public IActionResult HoSo()
        {
            // Lấy UserId từ Session (giống cách bạn đã dùng ở trên)
            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem hồ sơ";
                return RedirectToAction("Login", "Account");
            }

            // Lấy NguoiDungId từ bảng BenhNhan (vì BenhNhan.Id chính là NguoiDungId)
            var benhNhan = _context.BenhNhans
                .FirstOrDefault(b => b.Id == currentBenhNhanId.Value);

            if (benhNhan == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin bệnh nhân";
                return RedirectToAction("BenhNhanDashboard");
            }

            var hoSo = _benhNhanService.GetHoSo(benhNhan.Id);

            if (hoSo == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("BenhNhanDashboard");
            }

            return View(hoSo);
        }

        // POST: Cập nhật hồ sơ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoBenhNhanRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại";
                return RedirectToAction(nameof(HoSo));
            }

            // Lấy UserId từ Session
            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để cập nhật hồ sơ";
                return RedirectToAction("Login", "Account");
            }

            var benhNhan = _context.BenhNhans
                .FirstOrDefault(b => b.Id == currentBenhNhanId.Value);

            if (benhNhan == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin bệnh nhân";
                return RedirectToAction("BenhNhanDashboard");
            }

            var (success, message) = _benhNhanService.CapNhatHoSo(benhNhan.Id, request);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(ThongTinCaNhan));
        }
        // POST: Đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin";
                return RedirectToAction(nameof(HoSo));
            }

            int? currentBenhNhanId = HttpContext.Session.GetInt32("UserId");

            if (currentBenhNhanId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập";
                return RedirectToAction("Login", "Account");
            }

            var (success, message) = await _benhNhanService.DoiMatKhau(currentBenhNhanId.Value, request);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }
    }
}