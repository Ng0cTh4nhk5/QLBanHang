using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanHang.DAL.Entities
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaKH { get; set; }

        [Required]
        [StringLength(100)]
        public string TenKH { get; set; }

        [StringLength(20)]
        public string DienThoai { get; set; }

        [StringLength(200)]
        public string DiaChi { get; set; }
    }
}