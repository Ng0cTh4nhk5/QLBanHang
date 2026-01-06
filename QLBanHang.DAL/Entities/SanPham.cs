using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanHang.DAL.Entities // Khuyến khích gom vào namespace Entities
{
    [Table("SanPham")] // Ánh xạ tới bảng SanPham trong DB
    public class SanPham
    {
        [Key] // Khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự tăng
        public int MaSP { get; set; }

        [Required]
        [StringLength(100)]
        public string TenSP { get; set; }

        public decimal? DonGia { get; set; }

        public int? SoLuong { get; set; }

        public bool? TrangThai { get; set; }
    }
}