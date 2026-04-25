using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Models
{
    public class LichTruc
    {
        public int Id { get; set; }
        public int BacSiId { get; set; }
        public int PhongKhamId { get; set; }
        public DateOnly Ngay { get; set; } // Ngày bác sĩ trực

        // Quan hệ
        public virtual BacSi BacSi { get; set; }
        public virtual PhongKham PhongKham { get; set; }
    }
}
