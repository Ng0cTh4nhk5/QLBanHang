using System;
using System.Collections.Generic;
using System.Data; // Cần thiết để dùng DataTable
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBanHang.DAL;
using QLBanHang.DTO; // Nhớ using tầng DAL

namespace QLBanHang.BUS
{
    public class KhachHangBUS
    {
        // Khai báo đối tượng DAL để giao tiếp với CSDL
        private KhachHangDAL khachHangDAL;

        public KhachHangBUS()
        {
            khachHangDAL = new KhachHangDAL();
        }

        // Hàm lấy danh sách khách hàng
        // Trả về DataTable để dễ dàng gán vào DataSource của ComboBox/DataGridView
        public List<KhachHangDTO> LayDanhSachKhachHang()
        {
            return khachHangDAL.LayDanhSachKhachHang();
        }
    }
}