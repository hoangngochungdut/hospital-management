using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLyPhongKham.Models
{
    public class HoaDon
    {
        [Key]
        public int BuoiKhamId { get; set; }
        public BuoiKham? BuoiKham{ get; set; }

    }
}
