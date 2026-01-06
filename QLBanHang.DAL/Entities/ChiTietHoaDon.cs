using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanHang.DAL.Entities
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Key, Column(Order = 0)]
        public int MaHD { get; set; }

        [Key, Column(Order = 1)]
        public int MaSP { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }
    }
}