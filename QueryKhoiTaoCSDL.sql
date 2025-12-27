/* 
	DROP DATABASE QLBanHang
	GO
*/

 
/* Tạo database */
CREATE DATABASE QLBanHang;
GO

USE QLBanHang
GO


/* Tạo table */
-- Khách hàng
CREATE TABLE KhachHang (
	MaKH INT IDENTITY(1, 1) PRIMARY KEY,
	TenKH NVARCHAR(100) NOT NULL,
	DienThoai VARCHAR(20),
	DiaChi NVARCHAR(200)
);
GO
--- DROP TABLE KhachHang 
--- GO

-- Nhân viên
CREATE TABLE NhanVien (
	MaNV INT IDENTITY(1, 1) PRIMARY KEY,
	TenNV NVARCHAR(100) NOT NULL,
	ChucVu NVARCHAR(50),
	DienThoai VARCHAR(20)
);
GO
--- DROP TABLE NhanVien 
--- GO

-- Sản phẩm
CREATE TABLE SanPham (
	MaSP INT IDENTITY(1, 1) PRIMARY KEY,
	TenSP NVARCHAR(100) NOT NULL,
	DonGia DECIMAL(18, 0) DEFAULT 0,
	SoLuong INT DEFAULT 0,
	TrangThai BIT DEFAULT 1 -- 1 là còn kinh doanh, 0 là đang ngưng
);
GO
--- DROP TABLE SanPham 
--- GO

-- Hoá đơn
CREATE TABLE HoaDon (
	MaHD INT IDENTITY(1, 1) PRIMARY KEY,
	NgayLap DATETIME DEFAULT GETDATE(),
	MaNV INT NOT NULL,
	MaKH INT NOT NULL,

	CONSTRAINT FK_HoaDon_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
	CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);
GO
--- DROP TABLE HoaDon 
--- GO

-- Chi tiết hoá đơn
CREATE TABLE ChiTietHoaDon (
	MaHD INT NOT NULL,
	MaSP INT NOT NULL,
	SoLuong INT NOT NULL CHECK (SoLuong > 0),
	DonGia DECIMAL (18, 0) NOT NULL,

	CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHD, MaSP),
	CONSTRAINT FK_ChiTietHoaDon_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
	CONSTRAINT FK_ChiTietHoaDon_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO
--- DROP TABLE ChiTietHoaDon 
--- GO

/* Thêm dữ liệu mẫu */
-- Khách hàng
INSERT INTO KhachHang (TenKH, DienThoai, DiaChi) VALUES
(N'Nguyễn Văn A', '0901234567', N'Hà Nội'),
(N'Trần Thị B', '0912345678', N'Tp.HCM');
--- SELECT * FROM KhachHang

-- Nhân viên
INSERT INTO NhanVien (TenNV, ChucVu, DienThoai) VALUES
(N'Lê Quản Lý', N'Quản lý cửa hàng', '0988686868'),
(N'Phạm Nhân Viên', N'Nhân viên bán hàng', '0977777777');
--- SELECT * FROM NhanVien

-- Sản phẩm
INSERT INTO SanPham (TenSP, DonGia, SoLuong, TrangThai) VALUES
(N'Laptop Dell XPS', 25000000, 10, 1),
(N'Laptop Lenovo Thinkbook', 23000000, 8, 1),
(N'Bàn phím cơ', 1200000, 20, 1),
(N'Tai nghe Sony', 800000, 0, 1), -- Sản phẩm tồn kho = 0
(N'Loa JBL', 2000000, 5, 0); -- Sản phẩm ngừng kinh doanh
--- SELECT * FROM SanPham

SELECT * FROM HoaDon
GO