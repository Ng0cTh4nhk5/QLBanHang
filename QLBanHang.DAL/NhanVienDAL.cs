using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBanHang.DTO;

namespace QLBanHang.DAL
{
    public class NhanVienDAL
    {
        public List<NhanVienDTO> LayDanhSachKhachHang()
        {
            using (QLBanHangContextDataContext db = new QLBanHangContextDataContext())
            {
                return db.NhanViens.Select(nv => new NhanVienDTO
                {
                    MaNV = nv.MaNV,
                    TenNV = nv.TenNV
                }).ToList();
            }
        }
        // Em cần tự tạo class KhachHangDTO bên project DTO tương tự như SanPhamDTO nhé (chỉ cần MaKH, TenKH là đủ cho ComboBox)
    }
}