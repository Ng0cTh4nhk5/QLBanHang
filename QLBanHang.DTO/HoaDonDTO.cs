using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBanHang.DTO
{
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; }

        public int MaNV { get; set; }
        public string TenNV { get; set; } // Tên nhân viên

        public int MaKH { get; set; }
        public string TenKH { get; set; } // Tên khách hàng

        public decimal TongTien { get; set; }

        // Constructor
        public HoaDonDTO()
        {
            NgayLap = DateTime.Now;
            TongTien = 0; // Khởi tạo giá trị mặc định
        }
    }
}
