using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Interfaces
{
    public  interface IBenhNhanService
    {
        // Lấy hồ sơ bệnh nhân theo NguoiDungId
        XemHoSoBenhNhanResponse? GetHoSo(int nguoiDungId);
        //doi mat khau 
        Task<IEnumerable<BenhNhan>> GetAllAsync();
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoBenhNhanRequest request);
        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);
        ICollection<BenhNhan> GetAll();
        public void Add(AddBenhNhanDto entity);
        public BenhNhan? GetById(int id);
        BenhNhan? GetByIdWithTaiKhoan(int id);
        public void Update(int id, CapNhatHoSoBenhNhanRequest request);
        public void Delete(int id);


    }
}
