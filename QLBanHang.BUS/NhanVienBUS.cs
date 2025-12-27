using QLBanHang.DAL;
using QLBanHang.DTO; // Nhớ using DTO
using System.Collections.Generic;

namespace QLBanHang.BUS
{
    public class NhanVienBUS
    {
        // Khởi tạo đối tượng DAL
        private NhanVienDAL dal = new NhanVienDAL();

        // Hàm lấy danh sách nhân viên để gọi từ Form
        public List<NhanVienDTO> LayDanhSachNhanVien()
        {
            // Lưu ý: Trong file NhanVienDAL.cs bạn gửi, tên hàm đang là "LayDanhSachKhachHang"
            // nên tôi gọi đúng tên đó để chương trình không báo lỗi.
            // Sau này rảnh bạn nên vào DAL sửa lại thành "LayDanhSachNhanVien" cho chuẩn nhé.
            return dal.LayDanhSachNhanVien();
        }
    }
}