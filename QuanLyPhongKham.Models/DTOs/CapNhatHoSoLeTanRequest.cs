using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham.Models.DTOs
{
    public class CapNhatHoSoLeTanRequest
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string HoTen { get; set; } = string.Empty;

        public string? GioiTinh { get; set; }
        public string? DiaChi { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }
    }
}