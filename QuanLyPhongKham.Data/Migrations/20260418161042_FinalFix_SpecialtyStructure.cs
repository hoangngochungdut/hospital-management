using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinalFix_SpecialtyStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacSis_PhongKhams_PhongLamViecId",
                table: "BacSis");

            migrationBuilder.DropIndex(
                name: "IX_BacSis_PhongLamViecId",
                table: "BacSis");

            migrationBuilder.DropColumn(
                name: "PhongLamViecId",
                table: "BacSis");

            migrationBuilder.AddColumn<int>(
                name: "ChuyenKhoaId",
                table: "PhongKhams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhongKhams_ChuyenKhoaId",
                table: "PhongKhams",
                column: "ChuyenKhoaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhongKhams_ChuyenKhoas_ChuyenKhoaId",
                table: "PhongKhams",
                column: "ChuyenKhoaId",
                principalTable: "ChuyenKhoas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhongKhams_ChuyenKhoas_ChuyenKhoaId",
                table: "PhongKhams");

            migrationBuilder.DropIndex(
                name: "IX_PhongKhams_ChuyenKhoaId",
                table: "PhongKhams");

            migrationBuilder.DropColumn(
                name: "ChuyenKhoaId",
                table: "PhongKhams");

            migrationBuilder.AddColumn<int>(
                name: "PhongLamViecId",
                table: "BacSis",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BacSis_PhongLamViecId",
                table: "BacSis",
                column: "PhongLamViecId",
                unique: true,
                filter: "[PhongLamViecId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BacSis_PhongKhams_PhongLamViecId",
                table: "BacSis",
                column: "PhongLamViecId",
                principalTable: "PhongKhams",
                principalColumn: "Id");
        }
    }
}
