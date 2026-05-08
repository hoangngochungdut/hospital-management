using QuanLyPhongKham.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IChuyenKhoaRepository : IRepository<ChuyenKhoa>
    {
        // của đặt lịch khám, cần lấy danh sách bác sĩ theo chuyên khoa
        Task<List<ChuyenKhoa>> GetAllAsync();
        ICollection<ChuyenKhoa> GetAll();
    }
}
