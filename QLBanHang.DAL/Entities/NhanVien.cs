using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanHang.DAL.Entities
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Cấu hình tự tăng
        public int MaNV { get; set; }

        [Required]
        [StringLength(100)]
        public string TenNV { get; set; }

        [StringLength(50)]
        public string ChucVu { get; set; }

        [StringLength(20)] // Thống nhất độ dài sđt
        public string DienThoai { get; set; }
    }
}