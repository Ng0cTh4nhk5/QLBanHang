using System;
using System.Collections.Generic;
using System.Linq;
using QLBanHang.DTO;
using QLBanHang.DAL.Entities; // Sử dụng Entity mới tạo
// using QLBanHang.DAL; // Không dùng LINQ to SQL context cũ nữa

namespace QLBanHang.DAL
{
    public class SanPhamDAL
    {
        // 1. Lấy danh sách sản phẩm
        public List<SanPhamDTO> LayDanhSachSanPham()
        {
            using (var db = new QLBanHangDbContext())
            {
                // EF Query: Cú pháp vẫn tương tự LINQ nhưng chạy trên DbContext
                var danhSach = db.SanPhams.Select(sp => new SanPhamDTO
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    // Xử lý Nullable decimal/int an toàn hơn
                    DonGia = sp.DonGia ?? 0,
                    SoLuong = sp.SoLuong ?? 0,
                    TrangThai = sp.TrangThai ?? false
                }).ToList();

                return danhSach;
            }
        }

        // 2. Thêm sản phẩm
        public bool ThemSanPham(SanPhamDTO spMoi)
        {
            using (var db = new QLBanHangDbContext())
            {
                try
                {
                    var sp = new SanPham
                    {
                        TenSP = spMoi.TenSP,
                        DonGia = spMoi.DonGia,
                        SoLuong = spMoi.SoLuong,
                        TrangThai = spMoi.TrangThai
                    };

                    db.SanPhams.Add(sp); // Thay InsertOnSubmit bằng Add
                    db.SaveChanges();    // Thay SubmitChanges bằng SaveChanges
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        // 3. Sửa sản phẩm
        public bool SuaSanPham(SanPhamDTO spSua)
        {
            using (var db = new QLBanHangDbContext())
            {
                try
                {
                    // Tìm đối tượng cần sửa
                    var sp = db.SanPhams.Find(spSua.MaSP); // EF hỗ trợ hàm Find theo ID rất nhanh

                    if (sp != null)
                    {
                        sp.TenSP = spSua.TenSP;
                        sp.DonGia = spSua.DonGia;
                        sp.SoLuong = spSua.SoLuong;
                        sp.TrangThai = spSua.TrangThai;

                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        // 4. Xóa sản phẩm
        public bool XoaSanPham(int maSP)
        {
            using (var db = new QLBanHangDbContext())
            {
                try
                {
                    var sp = db.SanPhams.Find(maSP);
                    if (sp != null)
                    {
                        db.SanPhams.Remove(sp); // Thay DeleteOnSubmit bằng Remove
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public SanPham LaySanPhamTheoTen(string tenSP)
        {
            using (var db = new QLBanHangDbContext())
            {
                // Tìm chính xác theo tên (không phân biệt hoa thường)
                return db.SanPhams
                         .FirstOrDefault(sp => sp.TenSP.ToLower() == tenSP.ToLower());
            }
        }

        // Hàm cập nhật (Update) sản phẩm nếu đã có sẵn
        public bool CapNhatSanPham(SanPham sp)
        {
            using (var db = new QLBanHangDbContext())
            {
                // Phải attach đối tượng vào context hoặc tìm lại để update
                var dbSP = db.SanPhams.Find(sp.MaSP);
                if (dbSP == null) return false;

                dbSP.SoLuong = sp.SoLuong;
                dbSP.DonGia = sp.DonGia;
                // Các trường khác nếu cần...

                db.SaveChanges();
                return true;
            }
        }
    }
}