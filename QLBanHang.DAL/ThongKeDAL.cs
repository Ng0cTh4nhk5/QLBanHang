using System;
using System.Collections.Generic;
using System.Linq;
using QLBanHang.DAL.Entities; // Import Entity

namespace QLBanHang.DAL
{
    public class ThongKeDAL
    {
        // 1. Danh sách hóa đơn theo khoảng ngày
        public object LayDsHoaDonTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            using (var db = new QLBanHangDbContext())
            {
                var data = from hd in db.HoaDons
                           join kh in db.KhachHangs on hd.MaKH equals kh.MaKH
                           join nv in db.NhanViens on hd.MaNV equals nv.MaNV
                           where hd.NgayLap >= tuNgay && hd.NgayLap <= denNgay
                           select new
                           {
                               Mã_HĐ = hd.MaHD,
                               Ngày_Lập = hd.NgayLap,
                               Khách_Hàng = kh.TenKH,
                               Nhân_Viên = nv.TenNV,
                           };
                return data.ToList();
            }
        }

        // 2. Top 3 sản phẩm bán chạy nhất
        public object LayTop3SanPhamBanChay()
        {
            using (var db = new QLBanHangDbContext())
            {
                var data = from ct in db.ChiTietHoaDons
                           join sp in db.SanPhams on ct.MaSP equals sp.MaSP
                           group ct by new { sp.MaSP, sp.TenSP } into g
                           orderby g.Sum(x => x.SoLuong) descending
                           select new
                           {
                               Mã_SP = g.Key.MaSP,
                               Tên_Sản_Phẩm = g.Key.TenSP,
                               Tổng_Số_Lượng_Bán = g.Sum(x => x.SoLuong),
                               Tổng_Doanh_Thu = g.Sum(x => x.SoLuong * x.DonGia)
                           };

                return data.Take(3).ToList();
            }
        }

        // 3. Tổng doanh thu theo tháng
        public object LayDoanhThuTheoThang()
        {
            using (var db = new QLBanHangDbContext())
            {
                var data = from hd in db.HoaDons
                           where hd.NgayLap != null
                           // Lưu ý: Trong EF, truy cập .Month/.Year đôi khi cần Canonical Functions nếu database cũ
                           // Nhưng với SQL Server hiện đại thì viết như dưới vẫn ổn.
                           group hd by new { Thang = hd.NgayLap.Value.Month, Nam = hd.NgayLap.Value.Year } into g
                           orderby g.Key.Nam descending, g.Key.Thang descending
                           select new
                           {
                               Tháng = "Tháng " + g.Key.Thang + "/" + g.Key.Nam,
                               Số_Lượng_Đơn = g.Count(),
                               // Nếu muốn tính tổng tiền thì sum ở đây (cần join thêm bảng ChiTiet)
                           };

                return data.ToList();
            }
        }
    }
}