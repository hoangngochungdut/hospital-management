using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Models
{
    public class BacSi : NguoiDung
    {
        public int? ChuyenKhoaId { get; set; }
        public ChuyenKhoa? ChuyenKhoa { get; set; }

        public int? PhongLamViecId { get; set; }
        public PhongKham? PhongLamViec { get; set; }

        public ICollection<BuoiKham>? BuoiKhams { get; set; } = new List<BuoiKham>();
    }
}
