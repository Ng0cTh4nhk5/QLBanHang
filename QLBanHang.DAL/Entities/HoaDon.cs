using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanHang.DAL.Entities
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        public int MaHD { get; set; }

        public DateTime? NgayLap { get; set; }

        public int MaNV { get; set; } // Khóa ngoại (chưa cần map relation vội)

        public int MaKH { get; set; } // Khóa ngoại
    }
}