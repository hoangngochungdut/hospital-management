namespace QuanLyPhongKham.Models.DTOs
{
    public class XemHoSoLeTanResponse
    {
        public int NguoiDungId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? GioiTinh { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
    }
}