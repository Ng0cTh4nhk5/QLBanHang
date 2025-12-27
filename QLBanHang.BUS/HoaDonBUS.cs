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
        HoaDonDAL hdDAL = new HoaDonDAL();
        KhachHangDAL khDAL = new KhachHangDAL();
        // Nhớ khai báo NhanVienDAL nhé

        public List<KhachHangDTO> LayDanhSachKhachHang()
        {
            return khDAL.LayDanhSachKhachHang();
        }

        // Em viết thêm hàm lấy Nhân viên tương tự nhé

        public bool LuuHoaDon(HoaDonDTO hd, List<ChiTietHoaDonDTO> chiTiet)
        {
            // Kiểm tra: Phải có ít nhất 1 sản phẩm mới cho lưu
            if (chiTiet.Count == 0) return false;

            return hdDAL.LuuHoaDon(hd, chiTiet);
        }
    }
}
