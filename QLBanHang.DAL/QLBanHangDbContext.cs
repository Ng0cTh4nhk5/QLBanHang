using QLBanHang.DAL.Entities;
using System.Data.Entity;

namespace QLBanHang.DAL
{
    public class QLBanHangDbContext : DbContext
    {
        public QLBanHangDbContext() : base("name=QLBanHangConnectionString")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChiTietHoaDon>().HasKey(p => new { p.MaHD, p.MaSP });

            base.OnModelCreating(modelBuilder);
        }
    }
}