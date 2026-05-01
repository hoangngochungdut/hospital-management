using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLyPhongKham.Models.DTOs
{
    public class AddBacSiDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string? HoTen { get; set; }
        public string? GioiTinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }
        public string? DiaChi { get; set; }
        public int ChuyenKhoaId { get; set; }

        // phần thuộc TaiKhoan
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string? TenDangNhap { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string? MatKhau { get; set; }
    }
}
