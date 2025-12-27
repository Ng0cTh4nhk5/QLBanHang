using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Nhớ using DTO để dùng được class SanPhamDTO
using QLBanHang.DTO;

namespace QLBanHang.DAL
{
    public class SanPhamDAL
    {
        // 1. Lấy danh sách sản phẩm
        public List<SanPhamDTO> LayDanhSachSanPham()
        {
            // Khởi tạo context kết nối
            using (QLBanHangContextDataContext db = new QLBanHangContextDataContext())
            {
                // Truy vấn LINQ: Lấy từ bảng SanPhams trong DB
                var danhSach = db.SanPhams.Select(sp => new SanPhamDTO
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    DonGia = (decimal)sp.DonGia, // Ép kiểu nếu cần
                    SoLuong = (int)sp.SoLuong,
                    TrangThai = (bool)sp.TrangThai
                }).ToList();

                return danhSach;
            }
        }

        // 2. Thêm sản phẩm
        public bool ThemSanPham(SanPhamDTO spMoi)
        {
            using (QLBanHangContextDataContext db = new QLBanHangContextDataContext())
            {
                // Tạo đối tượng Entity từ DTO
                SanPham sp = new SanPham
                {
                    TenSP = spMoi.TenSP,
                    DonGia = spMoi.DonGia,
                    SoLuong = spMoi.SoLuong,
                    TrangThai = spMoi.TrangThai
                    // MaSP tự tăng nên không gán
                };

                db.SanPhams.InsertOnSubmit(sp); // Đánh dấu để thêm
                db.SubmitChanges(); // Lưu xuống CSDL
                return true;
            }
        }

        // 3. Sửa sản phẩm
        public bool SuaSanPham(SanPhamDTO spSua)
        {
            using (QLBanHangContextDataContext db = new QLBanHangContextDataContext())
            {
                // Tìm sản phẩm cần sửa theo MaSP
                SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == spSua.MaSP);
                if (sp != null)
                {
                    sp.TenSP = spSua.TenSP;
                    sp.DonGia = spSua.DonGia;
                    sp.SoLuong = spSua.SoLuong;
                    sp.TrangThai = spSua.TrangThai;

                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
        }

        // 4. Xóa sản phẩm
        public bool XoaSanPham(int maSP)
        {
            using (QLBanHangContextDataContext db = new QLBanHangContextDataContext())
            {
                SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == maSP);
                if (sp != null)
                {
                    db.SanPhams.DeleteOnSubmit(sp);
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
        }
    }
}
