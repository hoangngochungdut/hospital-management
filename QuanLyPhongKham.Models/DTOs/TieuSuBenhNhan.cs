using QuanLyPhongKham.Models.DTOs;
namespace QuanLyPhongKham.Models.DTOs;

public class TieuSuBenhNhan
{
    public XemHoSoBenhNhanResponse ThongTin { get; set; } = new();

    public string? TienSuBenh { get; set; }

    public List<LichSuKhamDTO> LichSuKham { get; set; } = new();
}
public class LichSuKhamDTO
{
    public DateTime NgayKham { get; set; }

    public string? KetQua { get; set; }
    public string? GhiChu { get; set; }
}