using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Models.DTOs;
using QuanLyPhongKham.Models.Enums;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Bắt buộc đăng nhập
    public class BuoiKhamController : ControllerBase
    {
        private readonly IBuoiKhamService _service;

        public BuoiKhamController(IBuoiKhamService service)
        {
            _service = service;
        }

        [HttpPost("dat-lich")]
        [Authorize(Roles = "BenhNhan,LeTan,Admin")]
        public async Task<IActionResult> DatLich([FromBody] DatLichRequest request)
        {
            try
            {
                // Bóc tách Id và Role từ JWT Token
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                string role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                var result = await _service.DatLichKhamAsync(request, userId, role);
                if (result) return Ok(new { message = "Đặt lịch thành công!" });

                return BadRequest(new { message = "Lỗi hệ thống khi lưu dữ liệu." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/trang-thai")]
        [Authorize(Roles = "BacSi,Admin,LeTan,BenhNhan")]
        public async Task<IActionResult> CapNhatTrangThai(int id, [FromBody] int trangThaiMoiId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                string role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                TrangThaiBuoiKham trangThai = (TrangThaiBuoiKham)trangThaiMoiId;

                var result = await _service.CapNhatTrangThaiAsync(id, trangThai, userId, role);
                if (result) return Ok(new { message = "Cập nhật trạng thái thành công!" });

                return BadRequest(new { message = "Lỗi hệ thống." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}