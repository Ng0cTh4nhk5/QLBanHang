using QLBanHang.DAL.Entities; // Namespace chứa các Entity mới (nếu có)
using System.Data.Entity;
// Nếu các bảng chưa tạo Entity mới thì nó sẽ dùng class cũ trong QLBanHang.DAL

namespace QLBanHang.DAL
{
    public class QLBanHangDbContext : DbContext
    {
        public QLBanHangDbContext() : base("name=QLBanHang.DAL.Properties.Settings.QLBanHangConnectionString")
        {
        }

        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        // --- ĐÂY LÀ PHẦN QUAN TRỌNG ĐỂ SỬA LỖI ---
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // 1. Cấu hình khóa chính cho SanPham
            modelBuilder.Entity<SanPham>().HasKey(p => p.MaSP);

            // 2. Cấu hình khóa chính cho KhachHang
            modelBuilder.Entity<KhachHang>().HasKey(p => p.MaKH);

            // 3. Cấu hình khóa chính cho NhanVien
            modelBuilder.Entity<NhanVien>().HasKey(p => p.MaNV);

            // 4. Cấu hình khóa chính cho HoaDon
            modelBuilder.Entity<HoaDon>().HasKey(p => p.MaHD);

            // 5. Cấu hình khóa chính cho ChiTietHoaDon (Bảng này đặc biệt vì có 2 khóa chính)
            // Lỗi của bạn chắc chắn nằm ở đây nếu không khai báo kỹ
            modelBuilder.Entity<ChiTietHoaDon>().HasKey(p => new { p.MaHD, p.MaSP });

            base.OnModelCreating(modelBuilder);
        }
    }
}