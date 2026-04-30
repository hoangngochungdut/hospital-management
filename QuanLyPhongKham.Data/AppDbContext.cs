using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Models;
namespace QuanLyPhongKham.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Danh sách các DbSet tương ứng với các file models
        public DbSet<Admin> Admins { get; set; }
        public DbSet<BacSi> BacSis { get; set; }
        public DbSet<BenhNhan> BenhNhans { get; set; }
        public DbSet<BuoiKham> BuoiKhams { get; set; }
        public DbSet<ChuyenKhoa> ChuyenKhoas { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<KetQuaKham> KetQuaKhams { get; set; }
        public DbSet<LeTan> LeTans { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<PhongKham> PhongKhams { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<TieuSuBenhAn> TieuSuBenhAns { get; set; }
        public DbSet<LichTruc> LichTrucs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. Cấu hình Chiến lược TPT (Tách bảng cho kế thừa) ---
            // Việc gọi ToTable với tên khác nhau cho các lớp kế thừa sẽ kích hoạt TPT
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDungs");
            modelBuilder.Entity<BenhNhan>().ToTable("BenhNhans");
            modelBuilder.Entity<BacSi>().ToTable("BacSis");
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<LeTan>().ToTable("LeTans");

            // --- 2. Cấu hình các quan hệ 1-1 ---

            // NguoiDung - TaiKhoan
            modelBuilder.Entity<NguoiDung>()
                .HasOne(n => n.TaiKhoan)
                .WithOne(t => t.NguoiDung)
                .HasForeignKey<TaiKhoan>(t => t.NguoiDungId);

            modelBuilder.Entity<BacSi>()
                .HasOne(bs => bs.ChuyenKhoa)
                .WithMany(ck => ck.BacSis)
                .HasForeignKey(bs => bs.ChuyenKhoaId)
                .OnDelete(DeleteBehavior.SetNull);

            // BenhNhan - TieuSuBenhAn
            modelBuilder.Entity<BenhNhan>()
                .HasOne(b => b.TieuSuBenhAn)
                .WithOne(t => t.BenhNhan)
                .HasForeignKey<TieuSuBenhAn>(t => t.BenhNhanId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa bệnh nhân thì xóa luôn tiểu sử

            // BuoiKham - KetQuaKham
            modelBuilder.Entity<BuoiKham>()
                .HasOne(b => b.KetQuaKham)
                .WithOne(k => k.BuoiKham)
                .HasForeignKey<KetQuaKham>(k => k.BuoiKhamId);

            // BuoiKham - HoaDon
            modelBuilder.Entity<BuoiKham>()
                .HasOne(b => b.HoaDon)
                .WithOne(h => h.BuoiKham)
                .HasForeignKey<HoaDon>(h => h.BuoiKhamId);

            // --- 3. Cấu hình các quan hệ 1-N và xử lý Cascade Paths ---

            // BuoiKham - BenhNhan (Nhiều buổi khám thuộc về một bệnh nhân)
            modelBuilder.Entity<BuoiKham>()
                .HasOne(b => b.BenhNhan)
                .WithMany(bn => bn.BuoiKhams)
                .HasForeignKey(b => b.BenhNhanId)
                .OnDelete(DeleteBehavior.Restrict); // Chặn xóa để tránh lỗi Multiple Cascade Paths

            // BuoiKham - BacSi (Nhiều buổi khám thực hiện bởi một bác sĩ)
            modelBuilder.Entity<BuoiKham>()
                .HasOne(b => b.BacSi)
                .WithMany(b => b.BuoiKhams)
                .HasForeignKey(b => b.BacSiId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


}

