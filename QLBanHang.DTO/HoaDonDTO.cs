using System;

namespace QLBanHang.DTO
{
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; }

        public int MaNV { get; set; }
        public string TenNV { get; set; } // Property phụ trợ để hiển thị lên Grid

        public int MaKH { get; set; }
        public string TenKH { get; set; } // Property phụ trợ để hiển thị lên Grid

        public decimal TongTien { get; set; }

        public HoaDonDTO()
        {
            NgayLap = DateTime.Now; // Mặc định là thời điểm tạo
        }
    }
}