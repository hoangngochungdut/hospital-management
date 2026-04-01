using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace QuanLyPhongKham.Models
{
    public class BuoiKham
    {
        [Key]
        public int Id { get; set; }
        public DateOnly Ngay { get; set; }
        public TimeOnly Gio { get; set; }
        public string? TrangThai { get; set; }
        public int BenhNhanId { get; set; }
        public BenhNhan? BenhNhan { get; set; }
        public int BacSiId { get; set; }
        public BacSi? BacSi { get; set; }
        public int PhongKhamId { get; set; }
        public PhongKham? PhongKham { get; set; }
        public int KetQuaKhamId { get; set; }
        public KetQuaKham? KetQuaKham { get; set; }
        public int? HoaDonId { get; set; }
        public HoaDon? HoaDon { get; set; }
        
    }
}
