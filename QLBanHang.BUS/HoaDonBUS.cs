using QLBanHang.DAL;
using QLBanHang.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBanHang.BUS
{
    public class HoaDonBUS
    {
        // Khởi tạo DAL
        HoaDonDAL dal = new HoaDonDAL();

        public bool LuuHoaDon(HoaDonDTO hd, List<ChiTietHoaDonDTO> listChiTiet)
        {
            // Kiểm tra nghiệp vụ (Validation)
            if (listChiTiet == null || listChiTiet.Count == 0)
                return false;

            // Gọi DAL để xử lý lưu xuống DB
            return dal.LuuHoaDon(hd, listChiTiet);
        }
    }
}