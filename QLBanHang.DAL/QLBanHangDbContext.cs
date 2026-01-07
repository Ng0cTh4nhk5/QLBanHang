using QLBanHang.DAL.Entities;
using System.Data.Entity;

namespace QLBanHang.DAL
{
    public class QLBanHangDbContext : DbContext
    {
        // Gợi ý: Nên để tên chuỗi kết nối trùng tên class trong App.config để đỡ phải truyền chuỗi string dài dòng
        public QLBanHangDbContext() : base("name=QLBanHangConnectionString")
        {
            // Tắt tính năng LazyLoading nếu không cần thiết để tránh lỗi vòng lặp khi serialize
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Nếu bạn đã dùng Attribute [Key] ở Entities thì không cần khai báo lại ở đây.
            // Chỉ giữ lại những cấu hình phức tạp nếu Attribute không làm được.

            // Ví dụ: Nếu Entity ChiTietHoaDon đã có [Key, Column(Order=...)] thì dòng dưới đây có thể bỏ.
            // Nhưng để chắc chắn 100%, mình giữ lại cấu hình khóa phức hợp:
            modelBuilder.Entity<ChiTietHoaDon>().HasKey(p => new { p.MaHD, p.MaSP });

            base.OnModelCreating(modelBuilder);
        }
    }
}