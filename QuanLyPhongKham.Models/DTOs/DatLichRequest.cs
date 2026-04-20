using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Models.DTOs
{
    public class DatLichRequest
    {
        public DateOnly Ngay { get; set; }
        public TimeOnly Gio { get; set; }
        public int BacSiId { get; set; }
        public int PhongKhamId { get; set; }
        public int? BenhNhanId { get; set; }
    }
}
