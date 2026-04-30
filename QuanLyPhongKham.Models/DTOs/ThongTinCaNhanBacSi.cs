using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Models.DTOs
{
    public class ThongTinCaNhanBacSi
    {
        public CapNhatHoSoBacSiRequest capNhatHoSo { get; set; }
        public DoiMatKhauRequest doiMatKhau { get; set; }

        public ThongTinCaNhanBacSi()
        {

        }
        public ThongTinCaNhanBacSi(CapNhatHoSoBacSiRequest _capNhatHoSo, DoiMatKhauRequest _doiMatKhau)
        {
            capNhatHoSo = _capNhatHoSo;
            doiMatKhau = _doiMatKhau;
        }
    }
}
