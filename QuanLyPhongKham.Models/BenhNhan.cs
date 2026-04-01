using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Models
{
    public class BenhNhan : NguoiDung
    {
        public TieuSuBenhAn? TieuSuBenhAn { get; set; }
        public ICollection<BuoiKham> BuoiKhams { get; set; } = new List<BuoiKham>();

    }
}
