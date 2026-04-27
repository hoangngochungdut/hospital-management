using QuanLyPhongKham.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLyPhongKham.Models
{
    public class BuoiKham
    {
        [Key]
        public int Id { get; set; }
        public DateOnly Ngay { get; set; }
        public TimeOnly Gio { get; set; }
        public TrangThaiBuoiKham TrangThai { get; set; }
        public int BenhNhanId { get; set; }
        public BenhNhan? BenhNhan { get; set; }
        public int BacSiId { get; set; }
        public BacSi? BacSi { get; set; }
        public int PhongKhamId { get; set; }
        public PhongKham? PhongKham { get; set; }
        public int? KetQuaKhamId { get; set; }
        public KetQuaKham? KetQuaKham { get; set; }
        public int? HoaDonId { get; set; }
        public HoaDon? HoaDon { get; set; }
        public string? GhiChuKetQua { get; set; }
        public int? DiemDanhGia { get; set; }
        public string? NhanXetCuaBenhNhan { get; set; }
        public string? ThongBaoChoBenhNhan { get; set; }
    }
}