using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBanHang.DTO
{
    public class SanPhamDTO
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public bool TrangThai { get; set; }

        public SanPhamDTO() { }

        public SanPhamDTO(int maSP, string tenSP, decimal donGia, int soLuong, bool trangThai)
        {
            this.MaSP = maSP;
            this.TenSP = tenSP;
            this.DonGia = donGia;
            this.SoLuong = soLuong;
            this.TrangThai = trangThai;
        }
    }
}
