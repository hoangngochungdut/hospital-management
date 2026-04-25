using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Implementations
{
    public class PhongKhamService : IPhongKhamService
    {
        private readonly IPhongKhamRepository _phongKhamRepo;

        public PhongKhamService(IPhongKhamRepository phongKhamRepo)
        {
            _phongKhamRepo = phongKhamRepo;
        }

        public async Task<IEnumerable<PhongKham>> GetByChuyenKhoaAsync(int chuyenKhoaId)
        {
            return await _phongKhamRepo.GetByChuyenKhoaAsync(chuyenKhoaId);
        }

        public async Task<IEnumerable<PhongKham>> GetAllAsync()
        {
            return await _phongKhamRepo.GetAllAsync();
        }
    }
}