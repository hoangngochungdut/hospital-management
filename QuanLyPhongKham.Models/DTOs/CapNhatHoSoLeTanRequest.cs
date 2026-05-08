using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham.Models; // Đảm bảo đã using namespace chứa class LeTan

namespace QuanLyPhongKham.Models.DTOs
{
    public class CapNhatHoSoLeTanRequest
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string? HoTen { get; set; } = string.Empty;

        public string? GioiTinh { get; set; }

        public string? DiaChi { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        // Khởi tạo rỗng để Model Binding của ASP.NET Core hoạt động
        public CapNhatHoSoLeTanRequest()
        {

        }

        // Khởi tạo từ Object LeTan có sẵn dưới Database lên
        public CapNhatHoSoLeTanRequest(LeTan leTan)
        {
            HoTen = leTan.HoTen;
            GioiTinh = leTan.GioiTinh;
            DiaChi = leTan.DiaChi;
            SoDienThoai = leTan.Sdt;
            Email = leTan.Email;
        }
    }
}