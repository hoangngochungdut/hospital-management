using System;
using System.Collections.Generic;
using System.Text;
using QuanLyPhongKham.Models.DTOs;

namespace QuanLyPhongKham.Services.Interfaces
{
    public  interface IBenhNhanService
    {
        // Lấy hồ sơ bệnh nhân theo NguoiDungId
        XemHoSoBenhNhanResponse? GetHoSo(int nguoiDungId);
        //doi mat khau 
        

        // Cập nhật hồ sơ bệnh nhân
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBenhNhanRequest request);
        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);
    }
}
