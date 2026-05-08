using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLyPhongKham.Models.DTOs
{
    public  class CapNhatHoSoBenhNhanRequest
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        public string? GioiTinh { get; set; }

        public string? DiaChi { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public CapNhatHoSoBenhNhanRequest()
        {

        }
        public CapNhatHoSoBenhNhanRequest(BenhNhan benhnhan)
        {
            HoTen = benhnhan.HoTen;
            GioiTinh = benhnhan.GioiTinh;
            DiaChi = benhnhan.DiaChi;
            SoDienThoai = benhnhan.Sdt;
            Email = benhnhan.Email;
        }
    }
}
