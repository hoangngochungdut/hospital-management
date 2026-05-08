using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;
using System.Security.Cryptography; // Để dùng hàm HMACSHA256 (tạo chữ ký)
using System.Text;               // Để mã hóa chuỗi sang Byte
using System.Net.Http;             // Để gửi dữ liệu sang máy chủ MoMo
using System.Text.Json;            // Để đọc kết quả trả về từ MoMo

namespace QuanLyPhongKham.Web.Controllers
{
    public class LeTanDashboardController : Controller
    {
        private readonly ILeTanService _leTanService;
        private readonly IBuoiKhamService _buoiKhamService;
        private readonly IBenhNhanService _benhNhanService;

        public LeTanDashboardController(
            ILeTanService leTanService,
            IBuoiKhamService buoiKhamService,
            IBenhNhanService benhNhanService)
        {
            _leTanService = leTanService;
            _buoiKhamService = buoiKhamService;
            _benhNhanService = benhNhanService;
        }

        // ==================== QUẢN LÝ LỊCH KHÁM (LỄ TÂN) ====================
        [HttpGet]
        public async Task<IActionResult> LichKham()
        {
            ViewBag.DsChuyenKhoa = await _buoiKhamService.LayTatCaChuyenKhoaAsync();
            ViewBag.DsBenhNhan = await _benhNhanService.GetAllAsync();

            var danhSachLich = await _buoiKhamService.LayToanBoLichKhamAdminAsync();

            return View(danhSachLich);
        }

        [HttpPost]
        public async Task<IActionResult> DatLich(
            int BenhNhanId, DateOnly Ngay, string Gio, int BacSiId, int PhongKhamId)
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
                PhongKhamId = PhongKhamId,
                BenhNhanId = BenhNhanId
            };

            try
            {
                var result = await _buoiKhamService.DatLichKhamAsync(request, currentId.Value, "LeTan");
                if (result) TempData["ThongBao"] = "✅ Đặt lịch hộ thành công (đã xác nhận)!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        [HttpPost]
        public IActionResult XacNhanLich(int id)
        {
            try
            {
                bool isSuccess = _buoiKhamService.XulyCaKham(id, TrangThaiBuoiKham.XacNhan, null, null);

                if (isSuccess) TempData["ThongBao"] = "✅ Đã xác nhận lịch khám thành công!";
                else TempData["ThongBao"] = "❌ Lỗi: Không tìm thấy lịch khám!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi xác nhận: " + ex.Message;
            }

            return RedirectToAction("QuanLyLichKham");
        }

        [HttpPost]
        public IActionResult HuyLichKham(int id, string lyDo)
        {
            try
            {
                bool isSuccess = _buoiKhamService.XulyCaKham(id, TrangThaiBuoiKham.Huy, lyDo, null);

                if (isSuccess) TempData["ThongBao"] = "✅ Đã hủy lịch khám!";
                else TempData["ThongBao"] = "❌ Lỗi: Không tìm thấy lịch khám!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi hủy lịch: " + ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        [HttpGet]
        public async Task<IActionResult> QuanLyLichKham()
        {
            var danhSachLich = await _buoiKhamService.LayToanBoLichKhamAdminAsync();
            return View(danhSachLich); 
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

        // ==================== HỒ SƠ ====================
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Account");

            var result = _leTanService.GetHoSo(id.Value);
            if (result == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin";
                return RedirectToAction("LeTanDashboard");
            }
            return View(result);
        }

        [HttpGet]
        public IActionResult HoSo()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Account");

            var result = _leTanService.GetHoSo(id.Value);
            if (result == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction("LeTanDashboard");
            }
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHoSo(CapNhatHoSoLeTanRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(HoSo));
            }

            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Account");

            var (success, message) = _leTanService.CapNhatHoSo(id.Value, request);
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
            if (id == null) return RedirectToAction("Login", "Account");

            if (request.MatKhauMoi != request.XacNhanMatKhauMoi)
            {
                TempData["Error"] = "Mật khẩu không khớp";
                return RedirectToAction(nameof(HoSo));
            }

            var (success, message) = await _leTanService.DoiMatKhau(id.Value, request);
            TempData[success ? "Success" : "Error"] = message;

            return RedirectToAction(nameof(HoSo));
        }
        private string CreateSignature(string rawHash, string secretKey)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(rawHash);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> ConfirmMomo(long sotien, int lichKhamId)
        {
            // Các thông tin Key giữ nguyên như cũ
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMOBKUN20180529";
            string accessKey = "klm05TvNBzhg7h7j";
            string secretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";

            string orderInfo = "Thanh toán phí khám bệnh #" + lichKhamId;
            string redirectUrl = "https://localhost:7282/LeTanDashboard/MomoCallback";
            string ipnUrl = "https://localhost:7282/LeTanDashboard/IPN";
            string requestType = "captureWallet";

            string orderId = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();

            // QUAN TRỌNG: Gửi ID lịch khám vào đây để MoMo trả ngược lại cho mình
            string extraData = lichKhamId.ToString();

            // Tạo chuỗi ký (Lưu ý: Phải có extraData trong chuỗi này)
            string rawHash = $"accessKey={accessKey}&amount={sotien}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";

            string signature = CreateSignature(rawHash, secretKey);

            var requestData = new MomoPaymentRequest
            {
                partnerCode = partnerCode,
                requestId = requestId,
                amount = sotien,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = redirectUrl,
                ipnUrl = ipnUrl,
                lang = "vi",
                extraData = extraData, // Đảm bảo extraData được đóng gói
                requestType = requestType,
                signature = signature
            };

            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(endpoint, requestData);
                var responseContent = await response.Content.ReadAsStringAsync();
                var momoResponse = JsonDocument.Parse(responseContent);

                if (momoResponse.RootElement.TryGetProperty("payUrl", out var payUrl))
                {
                    return Redirect(payUrl.GetString());
                }
                return BadRequest("Lỗi kết nối MoMo");
            }
        }
        [HttpGet]
        public async Task<IActionResult> MomoCallback(string resultCode, string message, string extraData)
        {
            // 1. Dùng resultCode thay vì errorCode
            // MoMo trả về "0" là giao dịch thành công hoàn toàn
            if (resultCode == "0")
            {
                if (int.TryParse(extraData, out int lichKhamId))
                {
                    try
                    {
                        // 2. PHẢI MỞ KHÓA DÒNG NÀY ĐỂ CẬP NHẬT DATABASE
                        // Gọi hàm cập nhật DaThanhToan = true mà mình đã làm ở các bước trước
                        _buoiKhamService.CapNhatThanhToan(lichKhamId);

                        TempData["SuccessMessage"] = $"✅ Thanh toán thành công cho ca khám #{lichKhamId}!";
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Lỗi cập nhật dữ liệu: " + ex.Message;
                    }
                }
            }
            else
            {
                // Nhảy vào đây nếu khách hủy hoặc resultCode != 0
                TempData["ErrorMessage"] = "Thanh toán thất bại hoặc đã bị hủy: " + message;
            }

            return RedirectToAction("QuanLyLichKham");
        }
        [HttpPost]
        public IActionResult XacNhanThuTien(int id)
        {
            try
            {
                // Dùng lại chính hàm chúng ta vừa fix lỗi xong!
                _buoiKhamService.CapNhatThanhToan(id);
                TempData["ThongBao"] = "✅ Đã xác nhận thu tiền mặt thành công!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "❌ Lỗi: " + ex.Message;
            }
            return RedirectToAction("QuanLyLichKham");
        }
    }
}