using QLBanHang.DAL;
using QLBanHang.DTO;
using System.Collections.Generic;

namespace QLBanHang.BUS
{
    public class NhanVienBUS
    {
        private readonly NhanVienDAL _nhanVienDAL;

        public NhanVienBUS()
        {
            _nhanVienDAL = new NhanVienDAL();
        }

        /// <summary>
        /// Lấy danh sách nhân viên
        /// </summary>
        public List<NhanVienDTO> LayDanhSachNhanVien()
        {
            return _nhanVienDAL.LayDanhSachNhanVien();
        }
    }
}