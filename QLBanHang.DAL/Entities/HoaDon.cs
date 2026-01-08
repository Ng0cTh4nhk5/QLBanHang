using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanHang.DAL.Entities
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Cấu hình tự tăng
        public int MaHD { get; set; }

        public DateTime NgayLap { get; set; } 

        public int MaNV { get; set; }

        public int MaKH { get; set; }
    }
}