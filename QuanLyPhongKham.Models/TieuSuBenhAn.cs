using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace QuanLyPhongKham.Models
{
    public class TieuSuBenhAn
    {
        [Key]
        public int BenhNhanId { get; set; }
        public string? MoTa { get; set; }
        public BenhNhan? BenhNhan { get; set; }
    }
}
