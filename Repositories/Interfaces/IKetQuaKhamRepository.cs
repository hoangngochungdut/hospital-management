using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IKetQuaKhamRepository
    {
        KetQuaKham GetById(int buoiKhamId);
        bool Add(KetQuaKham entity);
        bool Update(KetQuaKham entity);

    }
}