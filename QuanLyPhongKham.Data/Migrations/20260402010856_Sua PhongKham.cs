using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham.Data.Migrations
{
    /// <inheritdoc />
    public partial class SuaPhongKham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhongKhams_PhongKhams_PhongKhamId",
                table: "PhongKhams");

            migrationBuilder.DropIndex(
                name: "IX_PhongKhams_PhongKhamId",
                table: "PhongKhams");

            migrationBuilder.DropColumn(
                name: "PhongKhamId",
                table: "PhongKhams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhongKhamId",
                table: "PhongKhams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhongKhams_PhongKhamId",
                table: "PhongKhams",
                column: "PhongKhamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhongKhams_PhongKhams_PhongKhamId",
                table: "PhongKhams",
                column: "PhongKhamId",
                principalTable: "PhongKhams",
                principalColumn: "Id");
        }
    }
}
