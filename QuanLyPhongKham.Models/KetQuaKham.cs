using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace QuanLyPhongKham.Models
{
    public class KetQuaKham
    {
        [Key]
        public int BuoiKhamId { get; set; }
        public string? KetQua { get; set; }
        public BuoiKham? BuoiKham { get; set; }
    }
}
