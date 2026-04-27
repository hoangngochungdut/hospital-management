using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham.Data.Migrations
{
    /// <inheritdoc />
    public partial class CapNhatLuongBuoiKham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiemDanhGia",
                table: "BuoiKhams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChuKetQua",
                table: "BuoiKhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NhanXetCuaBenhNhan",
                table: "BuoiKhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThongBaoChoBenhNhan",
                table: "BuoiKhams",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiemDanhGia",
                table: "BuoiKhams");

            migrationBuilder.DropColumn(
                name: "GhiChuKetQua",
                table: "BuoiKhams");

            migrationBuilder.DropColumn(
                name: "NhanXetCuaBenhNhan",
                table: "BuoiKhams");

            migrationBuilder.DropColumn(
                name: "ThongBaoChoBenhNhan",
                table: "BuoiKhams");
        }
    }
}
