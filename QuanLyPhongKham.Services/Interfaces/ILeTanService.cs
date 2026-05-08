using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.DTOs;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface ILeTanService
    {
        XemHoSoLeTanResponse? GetHoSo(int nguoiDungId);
        (bool Success, string Message) CapNhatHoSo(int nguoiDungId, CapNhatHoSoLeTanRequest request);
        Task<(bool Success, string Message)> DoiMatKhau(int nguoiDungId, DoiMatKhauRequest request);
        LeTan? GetByIdWithTaiKhoan(int id);
        void Add(AddLeTanDto model);
        void Update(int id, CapNhatHoSoLeTanRequest model);
        void Delete(int id);
        ICollection<LeTan> GetAll();
    }
}