using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBanHang.DTO;

namespace QLBanHang.DAL
{
    public class KhachHangDAL
    {
        public List<KhachHangDTO> LayDanhSachKhachHang()
        {
            using (QLBanHangContextDataContext db = new QLBanHangContextDataContext())
            {
                return db.KhachHangs.Select(kh => new KhachHangDTO
                {
                    MaKH = kh.MaKH,
                    TenKH = kh.TenKH
                }).ToList();
            }
        }
        // Em cần tự tạo class KhachHangDTO bên project DTO tương tự như SanPhamDTO nhé (chỉ cần MaKH, TenKH là đủ cho ComboBox)
    }
}