using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLyPhongKham.Models
{
    public class ChuyenKhoa
    {
        [Key]
        public int Id { get; set; }
        public string? TenKhoa { get; set; }
        public ICollection<BacSi>? BacSis { get; set; }


    }
}
