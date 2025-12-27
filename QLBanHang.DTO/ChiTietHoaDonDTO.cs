using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBanHang.DTO
{
    public class ChiTietHoaDonDTO
    {
        public int MaHD { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } // Để hiển thị lên lưới
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get { return SoLuong * DonGia; } } // Tính tiền luôn tại đây
    }
}
