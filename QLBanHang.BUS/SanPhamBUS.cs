using QLBanHang.DAL;
using QLBanHang.DTO;
using System.Collections.Generic;
using System.Linq;

namespace QLBanHang.BUS
{
    public class SanPhamBUS
    {
        private readonly SanPhamDAL _sanPhamDAL;

        public SanPhamBUS()
        {
            _sanPhamDAL = new SanPhamDAL();
        }

        /// <summary>
        /// Lấy toàn bộ danh sách sản phẩm
        /// </summary>
        public List<SanPhamDTO> LayDanhSachSanPham()
        {
            return _sanPhamDAL.LayDanhSachSanPham();
        }

        /// <summary>
        /// Thêm mới sản phẩm có kiểm tra hợp lệ
        /// </summary>
        public bool ThemSanPham(SanPhamDTO sp)
        {
            if (string.IsNullOrEmpty(sp.TenSP)) return false;
            if (sp.DonGia < 0) return false;
            if (sp.SoLuong < 0) return false;

            return _sanPhamDAL.ThemSanPham(sp);
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        public bool SuaSanPham(SanPhamDTO sp)
        {
            if (sp.DonGia < 0 || sp.SoLuong < 0) return false;

            return _sanPhamDAL.SuaSanPham(sp);
        }

        /// <summary>
        /// Xóa sản phẩm theo mã
        /// </summary>
        public bool XoaSanPham(int maSP)
        {
            return _sanPhamDAL.XoaSanPham(maSP);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo tên (Lọc tại bộ nhớ bằng LINQ)
        /// </summary>
        public List<SanPhamDTO> TimKiemSanPham(string tuKhoa)
        {
            var ds = _sanPhamDAL.LayDanhSachSanPham();

            if (string.IsNullOrEmpty(tuKhoa))
                return ds;

            return ds.Where(x => x.TenSP.ToLower().Contains(tuKhoa.ToLower())).ToList();
        }

        /// <summary>
        /// Lấy chi tiết sản phẩm và chuyển đổi Entity sang DTO
        /// </summary>
        public SanPhamDTO LaySanPhamTheoTen(string tenSP)
        {
            var spEntity = _sanPhamDAL.LaySanPhamTheoTen(tenSP);
            if (spEntity == null) return null;

            // Mapping Entity -> DTO
            return new SanPhamDTO
            {
                MaSP = spEntity.MaSP,
                TenSP = spEntity.TenSP,
                DonGia = spEntity.DonGia,
                SoLuong = spEntity.SoLuong,
                TrangThai = spEntity.TrangThai
            };
        }
    }
}