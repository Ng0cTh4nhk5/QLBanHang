using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanHang.DAL.Entities 
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaSP { get; set; }

        [Required] // Tên sản phẩm bắt buộc phải có
        [StringLength(100)]
        public string TenSP { get; set; }

        // Refactor: Đổi sang non-nullable để tránh lỗi tính toán cộng trừ nhân chia
        public decimal DonGia { get; set; }

        public int SoLuong { get; set; }

        // Trạng thái: true = Đang bán, false = Ngừng bán
        public bool TrangThai { get; set; }
    }
}