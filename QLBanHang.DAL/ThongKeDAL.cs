using QLBanHang.DAL.Entities; // Sử dụng Entity mới
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace QLBanHang.DAL
{
    public class ThongKeDAL
    {
        // 1. Danh sách hóa đơn theo khoảng ngày
        public object LayDsHoaDonTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            using (var db = new QLBanHangDbContext())
            {
                // Lấy dữ liệu thô trước
                var data = from hd in db.HoaDons
                           join kh in db.KhachHangs on hd.MaKH equals kh.MaKH
                           join nv in db.NhanViens on hd.MaNV equals nv.MaNV
                           // So sánh ngày: cắt bỏ phần giờ phút giây để chính xác hơn
                           where DbFunctions.TruncateTime(hd.NgayLap) >= DbFunctions.TruncateTime(tuNgay)
                              && DbFunctions.TruncateTime(hd.NgayLap) <= DbFunctions.TruncateTime(denNgay)
                           select new
                           {
                               MaHD = hd.MaHD,
                               NgayLap = hd.NgayLap,
                               TenKH = kh.TenKH,
                               TenNV = nv.TenNV,
                               // Tính tổng tiền tạm tính cho hóa đơn (nếu cần)
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
                               MaSP = g.Key.MaSP,
                               TenSP = g.Key.TenSP,
                               SoLuong = g.Sum(x => x.SoLuong),
                               DoanhThu = g.Sum(x => x.SoLuong * x.DonGia)
                           };

                return data.Take(3).ToList();
            }
        }

        // 3. Tổng doanh thu theo tháng
        public object LayDoanhThuTheoThang()
        {
            using (var db = new QLBanHangDbContext())
            {
                // Lấy dữ liệu cần thiết về bộ nhớ trước (ToList) để tránh lỗi dịch SQL khi format chuỗi
                var rawData = db.HoaDons
                                .Where(h => h.NgayLap.HasValue)
                                .Select(h => new { h.NgayLap })
                                .ToList();

                // Xử lý Group By trong bộ nhớ (LINQ to Objects)
                var result = rawData
                             .GroupBy(x => new { Month = x.NgayLap.Value.Month, Year = x.NgayLap.Value.Year })
                             .OrderByDescending(g => g.Key.Year)
                             .ThenByDescending(g => g.Key.Month)
                             .Select(g => new
                             {
                                 ThoiGian = string.Format("Tháng {0}/{1}", g.Key.Month, g.Key.Year),
                                 SoLuongDon = g.Count()
                             })
                             .ToList();

                return result;
            }
        }
    }
}