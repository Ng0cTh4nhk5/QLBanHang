using QLBanHang.DAL;
using QLBanHang.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBanHang.BUS
{
    public class SanPhamBUS
    {
        private SanPhamDAL spDAL;

        public SanPhamBUS()
        {
            spDAL = new SanPhamDAL();
        }

        public List<SanPhamDTO> LayDanhSachSanPham()
        {
            return spDAL.LayDanhSachSanPham();
        }

        public bool ThemSanPham(SanPhamDTO sp)
        {
            if (string.IsNullOrEmpty(sp.TenSP))
            {
                return false; // Tên không được để trống
            }
            if (sp.DonGia < 0)
            {
                return false; // Đơn giá không được âm
            }
            if (sp.SoLuong < 0)
            {
                return false; // Số lượng không được âm
            }

            return spDAL.ThemSanPham(sp);
        }

        public bool SuaSanPham(SanPhamDTO sp)
        {
            // Kiểm tra logic trước khi sửa
            if (sp.DonGia < 0 || sp.SoLuong < 0) return false;

            return spDAL.SuaSanPham(sp);
        }

        public bool XoaSanPham(int maSP)
        {
            return spDAL.XoaSanPham(maSP);
        }
        public List<SanPhamDTO> TimKiemSanPham(string tuKhoa)
        {
            var ds = spDAL.LayDanhSachSanPham();
            // Sử dụng LINQ để lọc tại bộ nhớ (hoặc viết thêm hàm dưới DAL để tối ưu hơn)
            return ds.Where(x => x.TenSP.ToLower().Contains(tuKhoa.ToLower())).ToList();
        }

        // Thêm vào class SanPhamBUS
        public SanPhamDTO LaySanPhamTheoTen(string tenSP)
        {
            var spEntity = spDAL.LaySanPhamTheoTen(tenSP);
            if (spEntity == null) return null;

            // Chuyển từ Entity sang DTO
            return new SanPhamDTO
            {
                MaSP = spEntity.MaSP,
                TenSP = spEntity.TenSP,
                DonGia = spEntity.DonGia ?? 0,
                SoLuong = spEntity.SoLuong ?? 0,
                TrangThai = spEntity.TrangThai ?? false
            };
        }
    }
}
