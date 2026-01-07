using QLBanHang.DAL;
using QLBanHang.DTO;
using System.Collections.Generic;

namespace QLBanHang.BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL _hoaDonDAL;

        public HoaDonBUS()
        {
            _hoaDonDAL = new HoaDonDAL();
        }

        /// <summary>
        /// Lưu hóa đơn và chi tiết hóa đơn (Transaction)
        /// </summary>
        public bool LuuHoaDon(HoaDonDTO hd, List<ChiTietHoaDonDTO> listChiTiet)
        {
            // Kiểm tra dữ liệu đầu vào
            if (hd == null || listChiTiet == null || listChiTiet.Count == 0)
            {
                return false;
            }

            return _hoaDonDAL.LuuHoaDon(hd, listChiTiet);
        }

        /// <summary>
        /// Lấy dữ liệu để in hóa đơn (Report)
        /// </summary>
        public dsHoaDon LayDuLieuInHoaDon(int maHD)
        {
            if (maHD <= 0) return null;
            return _hoaDonDAL.LayDuLieuInHoaDon(maHD);
        }
    }
}