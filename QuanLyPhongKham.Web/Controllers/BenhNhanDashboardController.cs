using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;

namespace QuanLyPhongKham.Web.Controllers
{
    public class BenhNhanDashboardController : Controller
    {
        private readonly IBacSiService _bacSiService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly IBuoiKhamService _buoiKhamService;

        public BenhNhanDashboardController(
            IBacSiService bacSiService,
            IBenhNhanService benhNhanService,
            IBuoiKhamService buoiKhamService)
        {
            _bacSiService = bacSiService;
            _benhNhanService = benhNhanService;
            _buoiKhamService = buoiKhamService;
        }

        // ==================== ĐẶT LỊCH ====================
        [HttpGet]
        public async Task<IActionResult> LichKham()
        {
            ViewBag.DsChuyenKhoa = await _buoiKhamService.LayTatCaChuyenKhoaAsync();
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> DatLich(
        //    DateOnly Ngay,
        //    string Gio,
        //    int BacSiId,
        //    int PhongKhamId)
        //{
        //    int? currentId = HttpContext.Session.GetInt32("UserId");

        //    if (currentId == null)
        //        return RedirectToAction("Login", "Account");

        //    if (!TimeOnly.TryParse(Gio, out var gioParsed))
        //    {
        //        TempData["ThongBao"] = $"❌ Không đọc được giờ ({Gio})!";
        //        return RedirectToAction("LichKham");
        //    }

        //    var request = new DatLichRequest
        //    {
        //        Ngay = Ngay,
        //        Gio = gioParsed,
        //        BacSiId = BacSiId,
        //        PhongKhamId = PhongKhamId
        //    };

        //    try
        //    {
        //        var result = await _buoiKhamService.DatLichKhamAsync(request, currentId.Value, "BenhNhan");

        //        if (result)
        //            TempData["ThongBao"] = "✅ Đặt lịch thành công! Chờ xác nhận.";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ThongBao"] = ex.Message;
        //    }

        //    return RedirectToAction("LichKham");
        //}
        // ==================== ĐẶT LỊCH VÀ THANH TOÁN ====================
        [HttpPost]
        public async Task<IActionResult> DatLich(DateOnly Ngay, string Gio, int BacSiId, int PhongKhamId, string HinhThucThanhToan)
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");
            if (currentId == null) return RedirectToAction("Login", "Account");

            if (!TimeOnly.TryParse(Gio, out var gioParsed))
            {
                TempData["ThongBao"] = $"❌ Không đọc được giờ ({Gio})!";
                return RedirectToAction("LichKham");
            }

            var request = new DatLichRequest
            {
                Ngay = Ngay,
                Gio = gioParsed,
                BacSiId = BacSiId,
                PhongKhamId = PhongKhamId
            };

            try
            {
                // 1. Lưu lịch vào Database (Trạng thái mặc định: Chờ xác nhận, Chưa thanh toán)
                var result = await _buoiKhamService.DatLichKhamAsync(request, currentId.Value, "BenhNhan");

                if (result)
                {
                    // 2. Kiểm tra hình thức thanh toán bệnh nhân chọn
                    if (HinhThucThanhToan == "MoMo")
                    {
                        // Mẹo: Vì hàm DatLichKhamAsync trả về bool, ta lấy ca khám mới nhất vừa tạo của user này để lấy ID
                        var lichVuaTao = _buoiKhamService.GetByBenhNhanId(currentId.Value).OrderByDescending(x => x.Id).FirstOrDefault();

                        if (lichVuaTao != null)
                        {
                            // Chuyển hướng sang trang MoMo
                            return RedirectToAction("ConfirmMomo", new { sotien = 200000, lichKhamId = lichVuaTao.Id });
                        }
                    }

                    // Nếu chọn "Tại quầy" hoặc lỗi lấy ID MoMo
                    TempData["ThongBao"] = "✅ Đặt lịch thành công! Vui lòng thanh toán tại quầy khi đến khám.";
                    return RedirectToAction("XemLichKham");
                }
                else
                {
                    TempData["ThongBao"] = "❌ Hệ thống bận, không thể đặt lịch lúc này.";
                }
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi: " + ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        // ==================== TÍCH HỢP MOMO (BỆNH NHÂN) ====================
        [HttpGet]
        public async Task<IActionResult> ConfirmMomo(long sotien, int lichKhamId)
        {
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMOBKUN20180529";
            string accessKey = "klm05TvNBzhg7h7j";
            string secretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";

            string orderInfo = "Thanh toán phí khám bệnh #" + lichKhamId;
            string redirectUrl = "https://localhost:7282/BenhNhanDashboard/MomoCallback"; // Đổi thành BenhNhan
            string ipnUrl = "https://localhost:7282/BenhNhanDashboard/IPN";
            string requestType = "captureWallet";

            string orderId = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = lichKhamId.ToString(); // Đính kèm ID Lịch khám

            string rawHash = $"accessKey={accessKey}&amount={sotien}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";

            // Bạn copy hàm CreateSignature từ file Lễ Tân sang dưới cùng của class này nhé!
            string signature = CreateSignature(rawHash, secretKey);

            var requestData = new
            {
                partnerCode,
                requestId,
                amount = sotien,
                orderId,
                orderInfo,
                redirectUrl,
                ipnUrl,
                lang = "vi",
                extraData,
                requestType,
                signature
            };

            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(endpoint, requestData);
                var responseContent = await response.Content.ReadAsStringAsync();
                using (System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(responseContent))
                {
                    if (doc.RootElement.TryGetProperty("payUrl", out var payUrl))
                        return Redirect(payUrl.GetString());
                    return Content($"Lỗi kết nối MoMo: {responseContent}");
                }
            }
        }

        [HttpGet]
        public IActionResult MomoCallback(string errorCode, string message, string extraData)
        {
            if (errorCode == "0" && int.TryParse(extraData, out int lichKhamId))
            {
                // GỌI HÀM CẬP NHẬT TIỀN BẠC (Biến DaThanhToan = true)
                _buoiKhamService.CapNhatThanhToan(lichKhamId);

                TempData["ThongBao"] = $"✅ Thanh toán MoMo thành công cho lịch khám #{lichKhamId}!";
            }
            else
            {
                TempData["ThongBao"] = "❌ Thanh toán chưa hoàn tất: " + message;
            }
            return RedirectToAction("XemLichKham");
        }

        // ==================== AJAX ====================
        [HttpGet]
        public async Task<IActionResult> GetBacSiVaPhong(int chuyenKhoaId)
        {
            try
            {
                var data = await _buoiKhamService.LayBacSiVaPhongTheoKhoaAsync(chuyenKhoaId);
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGioTrong(int bacSiId, int phongKhamId, string ngay)
        {
            try
            {
                var date = DateOnly.Parse(ngay);
                var gioTrong = await _buoiKhamService.LayCacGioKhamTrongAsync(bacSiId, phongKhamId, date);
                return Json(new { success = true, gioTrong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message });
            }
        }

        // ==================== LỊCH CỦA TÔI ====================
        [HttpGet]
        public IActionResult XemLichKham()
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");

            if (currentId == null)
                return RedirectToAction("Login", "Account");

            var lich = _buoiKhamService.GetByBenhNhanId(currentId.Value);

            return View(lich);
        }

        [HttpPost]
        public IActionResult BenhNhanHuyLich(int id, string lyDo)
        {
            try
            {
                _buoiKhamService.XulyCaKham(id, TrangThaiBuoiKham.Huy, lyDo);
                TempData["ThongBao"] = "✅ Đã hủy lịch!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ " + ex.Message;
            }

            return RedirectToAction("XemLichKham");
        }

        [HttpPost]
        public IActionResult BenhNhanDoiLich(int id, DateTime ngayMoi, string gioMoi, string lyDo)
        {
            try
            {
                var date = DateOnly.FromDateTime(ngayMoi);
                var time = TimeOnly.Parse(gioMoi);

                _buoiKhamService.BenhNhanYeuCauDoiLich(id, date, time, lyDo);
                TempData["ThongBao"] = "✅ Đã gửi yêu cầu dời lịch!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ " + ex.Message;
            }

            return RedirectToAction("XemLichKham");
        }

        // ==================== ĐÁNH GIÁ ====================
        [HttpPost]
        public IActionResult NopDanhGia(int id, int soSao, string nhanXet)
        {
            try
            {
                _buoiKhamService.LuuDanhGiaCuaBenhNhan(id, soSao, nhanXet);
                TempData["ThongBao"] = "✅ Đánh giá thành công!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ " + ex.Message;
            }

            return RedirectToAction("XemLichKham");
        }

        // ==================== XEM BÁC SĨ ====================
        [HttpGet]
        public IActionResult XemBacSi()
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");

            if (currentId == null)
                return RedirectToAction("Login", "Account");

            var ds = _bacSiService.LayDanhSachBacSiVaDanhGia();

            return View(ds);
        }
        // 🔥 THÊM MỚI: API Lấy thông tin chi tiết Bác sĩ
        [HttpGet]
        public IActionResult GetThongTinBacSi(int id)
        {
            try
            {
                // Dùng lại hàm GetHoSo ông đã viết ở BacSiService
                var bacSi = _bacSiService.GetHoSo(id);

                if (bacSi == null) return Json(new { success = false });

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        hoTen = "BS. " + bacSi.HoTen,
                        gioiTinh = string.IsNullOrEmpty(bacSi.GioiTinh) ? "Chưa cập nhật" : bacSi.GioiTinh,
                        sdt = string.IsNullOrEmpty(bacSi.SoDienThoai) ? "Chưa cập nhật" : bacSi.SoDienThoai,
                        diaChi = string.IsNullOrEmpty(bacSi.DiaChi) ? "Chưa cập nhật" : bacSi.DiaChi
                    }
                });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        // ==================== HỒ SƠ ====================
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var hoSo = _benhNhanService.GetHoSo(id.Value);

            if (hoSo == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("BenhNhanDashboard");
            }

            return View(hoSo);
        }

        [HttpGet]
        public IActionResult HoSo()
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var hoSo = _benhNhanService.GetHoSo(id.Value);

            if (hoSo == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("BenhNhanDashboard");
            }

            return View(hoSo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoBenhNhanRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(HoSo));
            }

            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var (success, message) = _benhNhanService.CapNhatHoSo(id.Value, request);
            TempData[success ? "Success" : "Error"] = message;

            return RedirectToAction(nameof(ThongTinCaNhan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(HoSo));
            }

            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Account");

            var (success, message) = await _benhNhanService.DoiMatKhau(id.Value, request);
            TempData[success ? "Success" : "Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }
        private string CreateSignature(string text, string key)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

    }
}