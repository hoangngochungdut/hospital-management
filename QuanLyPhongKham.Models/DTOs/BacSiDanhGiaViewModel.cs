namespace QuanLyPhongKham.Models.DTOs
{
    public class BacSiDanhGiaViewModel
    {
        public int BacSiId { get; set; }
        public string? TenBacSi { get; set; }
        public string? TenChuyenKhoa { get; set; }
        public double DiemTrungBinh { get; set; }
        public int TongSoDanhGia { get; set; }
        // Danh sách chứa các lời nhận xét của bệnh nhân
        public List<ChiTietDanhGia> DanhSachNhanXet { get; set; } = new List<ChiTietDanhGia>();
    }

    public class ChiTietDanhGia
    {
        public int SoSao { get; set; }
        public string? NhanXet { get; set; }
        public string? TenBenhNhan { get; set; }
        public DateOnly NgayKham { get; set; }
    }
}