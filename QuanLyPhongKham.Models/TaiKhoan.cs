using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Models
{
    public class TaiKhoan
    {
        public int Id { get; set; }
        public int NguoiDungId { get; set; }
        public string? TenDangNhap { get; set; }
        public string? MatKhauHash { get; set; }
        public string? VaiTro { get; set; }
        public NguoiDung? NguoiDung{ get; set; }
    }
}
