using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class HoaDon
    {
        [Key]
        [ForeignKey("BuoiKham")]
        public int BuoiKhamId { get; set; }
        public BuoiKham? BuoiKham { get; set; }

        [Column(TypeName = "decimal(18,2)")] 
        public decimal TongTien { get; set; }

        public bool DaThanhToan { get; set; } = false; 

        public DateTime? NgayThanhToan { get; set; }

        public string? GhiChu { get; set; }
    }
}