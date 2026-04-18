using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLyPhongKham.Models
{
    public class PhongKham
    {
        [Key]
        public int Id { get; set; }
        public int SoPhong { get; set; }
        public int Tang { get; set; }
        public string? LoaiPhong { get; set; }
        public int? ChuyenKhoaId { get; set; }
        public ChuyenKhoa? ChuyenKhoa { get; set; }
        public ICollection<BuoiKham> BuoiKhams { get; set; } = new List<BuoiKham>();
    }
}

