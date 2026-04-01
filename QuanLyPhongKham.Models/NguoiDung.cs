using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLyPhongKham.Models
{
    public class NguoiDung
    {
        [Key]
        public int Id { get; set; }
        public string? HoTen { get; set; }
        public string? GioiTinh { get; set; }
        public string? DiaChi { get; set; }
        public string? Sdt { get; set; }

        public TaiKhoan? TaiKhoan { get; set; }
    }
}
