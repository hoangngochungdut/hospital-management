using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Models.DTOs
{
    public  class XemHoSoBenhNhanResponse
    {
        public int NguoiDungId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? GioiTinh { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
        
    }
}
