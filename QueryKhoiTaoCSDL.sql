/* ==========================================================
   SCRIPT KHỞI TẠO CSDL QUẢN LÝ BÁN HÀNG (FULL SAMPLE DATA)
   Created by: TA Môn LTCSDL
   Description: Tự động xóa DB cũ, tạo lại DB mới và insert dữ liệu mẫu
   ========================================================== */

USE master;
GO

-- 1. KIỂM TRA VÀ XÓA DATABASE CŨ (NẾU TỒN TẠI)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'QLBanHang')
BEGIN
    -- Chuyển về chế độ Single User để ngắt tất cả kết nối đang mở (Tránh lỗi "Database in use")
    ALTER DATABASE QLBanHang SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLBanHang;
    PRINT '>> Đã xóa CSDL QLBanHang cũ thành công.';
END
GO

-- 2. TẠO DATABASE MỚI
CREATE DATABASE QLBanHang;
GO

USE QLBanHang;
GO

PRINT '>> Đã tạo CSDL QLBanHang mới.';

/* ==========================================================
   TẠO CẤU TRÚC BẢNG (TABLES)
   ========================================================== */

-- Bảng 1: Khách hàng
CREATE TABLE KhachHang (
    MaKH INT IDENTITY(1, 1) PRIMARY KEY,
    TenKH NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(20),
    DiaChi NVARCHAR(200)
);
GO

-- Bảng 2: Nhân viên
CREATE TABLE NhanVien (
    MaNV INT IDENTITY(1, 1) PRIMARY KEY,
    TenNV NVARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50),
    DienThoai VARCHAR(20)
);
GO

-- Bảng 3: Sản phẩm
CREATE TABLE SanPham (
    MaSP INT IDENTITY(1, 1) PRIMARY KEY,
    TenSP NVARCHAR(100) NOT NULL,
    DonGia DECIMAL(18, 0) DEFAULT 0, -- Dùng Decimal cho tiền tệ (VND)
    SoLuong INT DEFAULT 0,
    TrangThai BIT DEFAULT 1 -- 1: Đang bán, 0: Ngừng kinh doanh
);
GO

-- Bảng 4: Hóa đơn
CREATE TABLE HoaDon (
    MaHD INT IDENTITY(1, 1) PRIMARY KEY,
    NgayLap DATETIME DEFAULT GETDATE(),
    MaNV INT NOT NULL,
    MaKH INT NOT NULL,

    -- Tạo khóa ngoại
    CONSTRAINT FK_HoaDon_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);
GO

-- Bảng 5: Chi tiết hóa đơn
CREATE TABLE ChiTietHoaDon (
    MaHD INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0), -- Số lượng mua phải > 0
    DonGia DECIMAL (18, 0) NOT NULL, -- Lưu giá tại thời điểm bán (đề phòng giá SP thay đổi sau này)

    -- Khóa chính phức hợp (Composite Key)
    CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHD, MaSP),
    
    -- Khóa ngoại
    CONSTRAINT FK_ChiTietHoaDon_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
    CONSTRAINT FK_ChiTietHoaDon_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO

PRINT '>> Đã tạo cấu trúc bảng thành công.';

/* ==========================================================
   INSERT DỮ LIỆU MẪU (SAMPLE DATA)
   ========================================================== */

-- 1. Insert Khách hàng
INSERT INTO KhachHang (TenKH, DienThoai, DiaChi) VALUES
(N'Nguyễn Văn An', '0901234567', N'123 Cầu Giấy, Hà Nội'),
(N'Trần Thị Bích', '0912345678', N'45 Lê Lợi, TP.HCM'),
(N'Lê Hoàng Cường', '0988777666', N'78 Nguyễn Trãi, Đà Nẵng');

-- 2. Insert Nhân viên
INSERT INTO NhanVien (TenNV, ChucVu, DienThoai) VALUES
(N'Admin Quản Lý', N'Quản lý', '0999999999'),
(N'Phạm Thu Ngân', N'Thu ngân', '0988888888'),
(N'Vũ Bán Hàng', N'Sale', '0977777777');

-- 3. Insert Sản phẩm
-- Lưu ý: ID sẽ tự tăng là 1, 2, 3, 4, 5
INSERT INTO SanPham (TenSP, DonGia, SoLuong, TrangThai) VALUES
(N'Laptop Dell XPS 13', 25000000, 10, 1),      -- ID: 1
(N'Laptop Lenovo ThinkPad', 23000000, 8, 1),   -- ID: 2
(N'Bàn phím cơ Logitech', 1500000, 20, 1),     -- ID: 3
(N'Chuột không dây Apple', 2000000, 15, 1),    -- ID: 4
(N'Tai nghe Sony (Hết hàng)', 800000, 0, 1);   -- ID: 5

-- 4. Insert Hóa đơn (Giả lập dữ liệu cho các tháng khác nhau để test thống kê)
-- ID tự tăng: 1, 2, 3

-- Hóa đơn 1: Mua tháng trước (Test thống kê theo tháng)
INSERT INTO HoaDon (NgayLap, MaNV, MaKH) VALUES ('2023-12-15', 2, 1); 

-- Hóa đơn 2: Mua hôm qua
INSERT INTO HoaDon (NgayLap, MaNV, MaKH) VALUES (DATEADD(day, -1, GETDATE()), 3, 2);

-- Hóa đơn 3: Mua hôm nay
INSERT INTO HoaDon (NgayLap, MaNV, MaKH) VALUES (GETDATE(), 2, 1);

-- 5. Insert Chi tiết hóa đơn
-- Lưu ý: Giá bán ở đây nên khớp hoặc khác giá gốc một chút (nếu có chiết khấu), nhưng để đơn giản ta lấy bằng giá gốc.

-- Chi tiết cho Hóa đơn 1 (Khách A mua Laptop Dell + Chuột)
INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(1, 1, 1, 25000000), -- 1 Laptop Dell
(1, 4, 1, 2000000);  -- 1 Chuột Apple

-- Chi tiết cho Hóa đơn 2 (Khách B mua 2 Bàn phím)
INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(2, 3, 2, 1500000);  -- 2 Bàn phím (Tổng 3tr)

-- Chi tiết cho Hóa đơn 3 (Khách A quay lại mua Laptop Lenovo)
INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(3, 2, 1, 23000000); -- 1 Laptop Lenovo

PRINT '>> Đã thêm dữ liệu mẫu thành công.';
GO

-- SELECT KIỂM TRA
SELECT * FROM SanPham;
SELECT * FROM HoaDon;
SELECT * FROM ChiTietHoaDon;