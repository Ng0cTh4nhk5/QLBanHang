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
        public string TenNV { get; set; } // Thêm tên để hiển thị cho dễ
        public int MaKH { get; set; }
        public string TenKH { get; set; } // Thêm tên để hiển thị cho dễ

        // Constructor
        public HoaDonDTO() { NgayLap = DateTime.Now; }
    }
}
