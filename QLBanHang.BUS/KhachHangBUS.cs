using QLBanHang.DAL;
using QLBanHang.DTO;
using System.Collections.Generic;

namespace QLBanHang.BUS
{
    public class KhachHangBUS
    {
        private readonly KhachHangDAL _khachHangDAL;

        public KhachHangBUS()
        {
            _khachHangDAL = new KhachHangDAL();
        }

        /// <summary>
        /// Lấy danh sách khách hàng (phục vụ ComboBox hoặc Grid)
        /// </summary>
        public List<KhachHangDTO> LayDanhSachKhachHang()
        {
            return _khachHangDAL.LayDanhSachKhachHang();
        }
    }
}