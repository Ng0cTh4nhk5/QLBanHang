using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBanHang.DTO
{
    public class KhachHangDTO
    {
        public int MaKH { get; set; }
        public string TenKH { get; set; }
        public KhachHangDTO() { }
        public KhachHangDTO(int maKH, string tenKH)
        {
            this.MaKH = maKH;
            this.TenKH = tenKH;
        }
    }
}
