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
            // Gọi xuống DAL. 
            // Lưu ý: Nếu DAL chưa sửa tên hàm "LayDanhSachKhachHang", hãy giữ nguyên hoặc vào DAL sửa lại cho đúng chuẩn.
            return _nhanVienDAL.LayDanhSachNhanVien();
        }
    }
}