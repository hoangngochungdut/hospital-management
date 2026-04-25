using QuanLyPhongKham.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IPhongKhamService
    {
        Task<IEnumerable<PhongKham>> GetByChuyenKhoaAsync(int chuyenKhoaId);
        Task<IEnumerable<PhongKham>> GetAllAsync();
    }
}