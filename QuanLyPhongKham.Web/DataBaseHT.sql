-- 1. XÓA DB CŨ NẾU CÓ VÀ TẠO LẠI DB MỚI TINH
USE master;
GO
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'HeThongLichKham')
BEGIN
    ALTER DATABASE HeThongLichKham SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HeThongLichKham;
END
GO

CREATE DATABASE HeThongLichKham;
GO
USE HeThongLichKham;
GO

-- ==========================================
-- PHẦN 1: TẠO 11 BẢNG
-- ==========================================
CREATE TABLE NguoiDung (
    nguoidung_id INT PRIMARY KEY IDENTITY(1,1),
    hoten NVARCHAR(255) NOT NULL,
    gioitinh CHAR(1),
    diachi INT,
    sodienthoai VARCHAR(10),
    role VARCHAR(2)
);

CREATE TABLE TaiKhoan (
    mataikhoan INT PRIMARY KEY IDENTITY(1,1),
    tentaikhoan VARCHAR(255) UNIQUE NOT NULL,
    matkhau VARCHAR(255) NOT NULL,
    nguoidung_id INT,
    CONSTRAINT FK_TaiKhoan_NguoiDung FOREIGN KEY (nguoidung_id) REFERENCES NguoiDung(nguoidung_id)
);

CREATE TABLE ChuyenKhoa (
    makhoa INT PRIMARY KEY IDENTITY(1,1),
    tenkhoa NVARCHAR(255) NOT NULL
);

CREATE TABLE BacSi (
    nguoidung_id INT PRIMARY KEY,
    makhoa INT,
    CONSTRAINT FK_BacSi_NguoiDung FOREIGN KEY (nguoidung_id) REFERENCES NguoiDung(nguoidung_id),
    CONSTRAINT FK_BacSi_ChuyenKhoa FOREIGN KEY (makhoa) REFERENCES ChuyenKhoa(makhoa)
);

CREATE TABLE BenhNhan (
    nguoidung_id INT PRIMARY KEY,
    CONSTRAINT FK_BenhNhan_NguoiDung FOREIGN KEY (nguoidung_id) REFERENCES NguoiDung(nguoidung_id)
);

CREATE TABLE TieuSuBenhAn (
    nguoidung_id INT PRIMARY KEY,
    mota NVARCHAR(255),
    CONSTRAINT FK_TieuSu_BenhNhan FOREIGN KEY (nguoidung_id) REFERENCES BenhNhan(nguoidung_id)
);

CREATE TABLE PhongKham (
    maphong INT PRIMARY KEY IDENTITY(1,1),
    BacSinguoidung_id INT,
    tang INT,
    sophong INT,
    CONSTRAINT FK_PhongKham_BacSi FOREIGN KEY (BacSinguoidung_id) REFERENCES BacSi(nguoidung_id)
);

CREATE TABLE BuoiKham (
    mabuoiKham INT PRIMARY KEY IDENTITY(1,1),
    mabacsi INT,
    mabenhnhan INT,
    ngaykham DATE,
    giokham TIME(7),
    trangthai INT,
    CONSTRAINT FK_BuoiKham_BacSi FOREIGN KEY (mabacsi) REFERENCES BacSi(nguoidung_id),
    CONSTRAINT FK_BuoiKham_BenhNhan FOREIGN KEY (mabenhnhan) REFERENCES BenhNhan(nguoidung_id)
);

CREATE TABLE KetQuaKham (
    mabuoiKham INT PRIMARY KEY,
    mota NVARCHAR(255),
    CONSTRAINT FK_KetQua_BuoiKham FOREIGN KEY (mabuoiKham) REFERENCES BuoiKham(mabuoiKham)
);

CREATE TABLE HoaDon (
    mahoadon INT PRIMARY KEY IDENTITY(1,1),
    mabuoiKham INT,
    thanhtien INT,
    CONSTRAINT FK_HoaDon_BuoiKham FOREIGN KEY (mabuoiKham) REFERENCES BuoiKham(mabuoiKham)
);

-- ==========================================
-- PHẦN 2: BƠM DỮ LIỆU MẪU VÀO ĐÚNG NHÀ
-- ==========================================
INSERT INTO ChuyenKhoa (tenkhoa) VALUES (N'Nội Tổng Quát'), (N'Nhi Khoa'), (N'Răng Hàm Mặt');

INSERT INTO NguoiDung (hoten, gioitinh, sodienthoai, role) VALUES 
(N'BS. Nguyễn Văn An', 'M', '0901234567', 'BS'),
(N'BS. Lê Thị Bình', 'F', '0902345678', 'LT'),
(N'Trần Văn Cường', 'M', '0903456789', 'AD'),
(N'Phạm Thu Thảo', 'F', '0904567890', 'BS');

INSERT INTO BacSi (nguoidung_id, makhoa) VALUES (1, 1), (2, 3);
INSERT INTO BenhNhan (nguoidung_id) VALUES (3), (4);

INSERT INTO BuoiKham (mabacsi, mabenhnhan, ngaykham, giokham, trangthai) VALUES 
(1, 3, '2026-04-01', '08:00:00', 1),
(2, 4, '2026-04-01', '09:30:00', 1);

INSERT INTO KetQuaKham (mabuoiKham, mota) VALUES 
(1, N'Cảm cúm nhẹ'), 
(2, N'Kiểm tra răng định kỳ');

INSERT INTO HoaDon (mabuoiKham, thanhtien) VALUES 
(1, 200000), 
(2, 500000);

-- XEM THỬ THÀNH QUẢ
SELECT * FROM TaiKhoan;
