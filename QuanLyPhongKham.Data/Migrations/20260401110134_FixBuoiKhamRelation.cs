using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBuoiKhamRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuyenKhoas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhoa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenKhoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sdt = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhongKhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<int>(type: "int", nullable: false),
                    Tang = table.Column<int>(type: "int", nullable: false),
                    LoaiPhong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhongKhamId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongKhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhongKhams_PhongKhams_PhongKhamId",
                        column: x => x.PhongKhamId,
                        principalTable: "PhongKhams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_NguoiDungs_Id",
                        column: x => x.Id,
                        principalTable: "NguoiDungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BenhNhans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BenhNhans_NguoiDungs_Id",
                        column: x => x.Id,
                        principalTable: "NguoiDungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeTans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeTans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeTans_NguoiDungs_Id",
                        column: x => x.Id,
                        principalTable: "NguoiDungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    TenDangNhap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaiKhoans_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BacSis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ChuyenKhoaId = table.Column<int>(type: "int", nullable: true),
                    PhongLamViecId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacSis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BacSis_ChuyenKhoas_ChuyenKhoaId",
                        column: x => x.ChuyenKhoaId,
                        principalTable: "ChuyenKhoas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BacSis_NguoiDungs_Id",
                        column: x => x.Id,
                        principalTable: "NguoiDungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BacSis_PhongKhams_PhongLamViecId",
                        column: x => x.PhongLamViecId,
                        principalTable: "PhongKhams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TieuSuBenhAns",
                columns: table => new
                {
                    BenhNhanId = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TieuSuBenhAns", x => x.BenhNhanId);
                    table.ForeignKey(
                        name: "FK_TieuSuBenhAns_BenhNhans_BenhNhanId",
                        column: x => x.BenhNhanId,
                        principalTable: "BenhNhans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuoiKhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BenhNhanId = table.Column<int>(type: "int", nullable: false),
                    BacSiId = table.Column<int>(type: "int", nullable: false),
                    PhongKhamId = table.Column<int>(type: "int", nullable: false),
                    KetQuaKhamId = table.Column<int>(type: "int", nullable: false),
                    HoaDonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuoiKhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuoiKhams_BacSis_BacSiId",
                        column: x => x.BacSiId,
                        principalTable: "BacSis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuoiKhams_BenhNhans_BenhNhanId",
                        column: x => x.BenhNhanId,
                        principalTable: "BenhNhans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuoiKhams_PhongKhams_PhongKhamId",
                        column: x => x.PhongKhamId,
                        principalTable: "PhongKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    BuoiKhamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.BuoiKhamId);
                    table.ForeignKey(
                        name: "FK_HoaDons_BuoiKhams_BuoiKhamId",
                        column: x => x.BuoiKhamId,
                        principalTable: "BuoiKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaKhams",
                columns: table => new
                {
                    BuoiKhamId = table.Column<int>(type: "int", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaKhams", x => x.BuoiKhamId);
                    table.ForeignKey(
                        name: "FK_KetQuaKhams_BuoiKhams_BuoiKhamId",
                        column: x => x.BuoiKhamId,
                        principalTable: "BuoiKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BacSis_ChuyenKhoaId",
                table: "BacSis",
                column: "ChuyenKhoaId");

            migrationBuilder.CreateIndex(
                name: "IX_BacSis_PhongLamViecId",
                table: "BacSis",
                column: "PhongLamViecId",
                unique: true,
                filter: "[PhongLamViecId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BuoiKhams_BacSiId",
                table: "BuoiKhams",
                column: "BacSiId");

            migrationBuilder.CreateIndex(
                name: "IX_BuoiKhams_BenhNhanId",
                table: "BuoiKhams",
                column: "BenhNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_BuoiKhams_PhongKhamId",
                table: "BuoiKhams",
                column: "PhongKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_PhongKhams_PhongKhamId",
                table: "PhongKhams",
                column: "PhongKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_NguoiDungId",
                table: "TaiKhoans",
                column: "NguoiDungId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "KetQuaKhams");

            migrationBuilder.DropTable(
                name: "LeTans");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "TieuSuBenhAns");

            migrationBuilder.DropTable(
                name: "BuoiKhams");

            migrationBuilder.DropTable(
                name: "BacSis");

            migrationBuilder.DropTable(
                name: "BenhNhans");

            migrationBuilder.DropTable(
                name: "ChuyenKhoas");

            migrationBuilder.DropTable(
                name: "PhongKhams");

            migrationBuilder.DropTable(
                name: "NguoiDungs");
        }
    }
}
