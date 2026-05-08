using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface ILeTanRepository : IRepository<LeTan>
    {
        // Thêm dòng này vào trong interface ILeTanRepository
        LeTan? GetByIdWithTaiKhoan(int id);
    }
}