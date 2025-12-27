using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBanHang.DTO
{
    public class NhanVienDTO
    {
        public int MaNV { get; set; }
        public string TenNV { get; set; }
        public NhanVienDTO() { }
        public NhanVienDTO(int maNV, string tenNV)
        {
            this.MaNV = maNV;
            this.TenNV = tenNV;
        }
    }
}
