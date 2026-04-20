using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Models.DTOs
{
    public  class XemHoSoBacSiResponse
    {
        public int NguoiDungId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? GioiTinh { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }

        // Thông tin riêng của Bác sĩ (từ bảng BacSi)
        public int ChuyenKhoaId { get; set; }
        public string? TenChuyenKhoa { get; set; }
        public int PhongLamViecId { get; set; }
        public string? TenPhong { get; set; }
    }
}
