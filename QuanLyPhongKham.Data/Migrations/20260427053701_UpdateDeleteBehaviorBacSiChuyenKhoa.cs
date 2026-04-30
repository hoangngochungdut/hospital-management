using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviorBacSiChuyenKhoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacSis_ChuyenKhoas_ChuyenKhoaId",
                table: "BacSis");

            migrationBuilder.DropForeignKey(
                name: "FK_BuoiKhams_BacSis_BacSiId",
                table: "BuoiKhams");

            migrationBuilder.AlterColumn<int>(
                name: "BenhNhanId",
                table: "BuoiKhams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BacSiId",
                table: "BuoiKhams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BacSis_ChuyenKhoas_ChuyenKhoaId",
                table: "BacSis",
                column: "ChuyenKhoaId",
                principalTable: "ChuyenKhoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BuoiKhams_BacSis_BacSiId",
                table: "BuoiKhams",
                column: "BacSiId",
                principalTable: "BacSis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacSis_ChuyenKhoas_ChuyenKhoaId",
                table: "BacSis");

            migrationBuilder.DropForeignKey(
                name: "FK_BuoiKhams_BacSis_BacSiId",
                table: "BuoiKhams");

            migrationBuilder.AlterColumn<int>(
                name: "BenhNhanId",
                table: "BuoiKhams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BacSiId",
                table: "BuoiKhams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BacSis_ChuyenKhoas_ChuyenKhoaId",
                table: "BacSis",
                column: "ChuyenKhoaId",
                principalTable: "ChuyenKhoas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BuoiKhams_BacSis_BacSiId",
                table: "BuoiKhams",
                column: "BacSiId",
                principalTable: "BacSis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
