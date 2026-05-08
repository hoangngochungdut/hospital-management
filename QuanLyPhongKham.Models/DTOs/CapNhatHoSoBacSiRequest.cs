//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Text;

//namespace QuanLyPhongKham.Models.DTOs
//{
//    public  class CapNhatHoSoBacSiRequest
//    {
//        [Required(ErrorMessage = "Họ tên không được để trống")]
//        public string HoTen { get; set; } = string.Empty;

//        public string? GioiTinh { get; set; }
//        public string? DiaChi { get; set; }

//        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
//        public string? SoDienThoai { get; set; }

//        // Thông tin riêng của Bác sĩ
//        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa")]
//        public int ChuyenKhoaId { get; set; }

//        [Required(ErrorMessage = "Vui lòng chọn phòng làm việc")]
//        public int PhongLamViecId { get; set; }
//    }
//}
using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham.Models.DTOs
{
    public class CapNhatHoSoBacSiRequest
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string? HoTen { get; set; } = string.Empty;

        public string? GioiTinh { get; set; }

        public string? DiaChi { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email{ get; set; }

        // Chỉ giữ lại chuyên khoa (vì bạn không dùng phòng khám)
        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa")]
        public int? ChuyenKhoaId { get; set; }

        public CapNhatHoSoBacSiRequest()
        {

        }
        public CapNhatHoSoBacSiRequest(BacSi bacSi)
        {
            HoTen = bacSi.HoTen;
            GioiTinh = bacSi.GioiTinh;
            DiaChi = bacSi.DiaChi;
            SoDienThoai = bacSi.Sdt;
            Email = bacSi.Email;
            ChuyenKhoaId = bacSi.ChuyenKhoaId;
        }
    }
}