using QLBanHang.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace QLBanHang.DAL
{
    // --- KHAI BÁO CÁC DTO (Class chứa dữ liệu) ĐỂ GUI CÓ THỂ ĐỌC ĐƯỢC ---
    public class BaoCaoTopSanPham
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DoanhThu { get; set; }

    }

    public class BaoCaoDoanhThu
    {
        public string ThoiGian { get; set; }
        public int SoLuongDon { get; set; }

        public decimal TongDoanhThu { get; set; }
        // Bạn có thể thêm property TongTien nếu muốn tính tổng doanh thu
    }
    // ---------------------------------------------------------------------

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
                           where DbFunctions.TruncateTime(hd.NgayLap) >= DbFunctions.TruncateTime(tuNgay)
                              && DbFunctions.TruncateTime(hd.NgayLap) <= DbFunctions.TruncateTime(denNgay)
                           // Nên dùng class cụ thể, nhưng nếu tab này đang chạy ổn thì tôi giữ nguyên logic
                           // Tuy nhiên, tốt nhất vẫn nên tạo class BaoCaoHoaDon tương tự như trên
                           select new
                           {
                               MaHD = hd.MaHD,
                               NgayLap = hd.NgayLap,
                               TenKH = kh.TenKH,
                               TenNV = nv.TenNV
                           };
                return data.ToList();
            }
        }

        // 2. Top 3 sản phẩm bán chạy nhất (Đã sửa để dùng class BaoCaoTopSanPham)
        public object LayTop3SanPhamBanChay()
        {
            using (var db = new QLBanHangDbContext())
            {
                var data = from ct in db.ChiTietHoaDons
                           join sp in db.SanPhams on ct.MaSP equals sp.MaSP
                           group ct by new { sp.MaSP, sp.TenSP } into g
                           orderby g.Sum(x => x.SoLuong) descending
                           // SỬA Ở ĐÂY: Dùng class BaoCaoTopSanPham thay vì new { ... }
                           select new BaoCaoTopSanPham
                           {
                               MaSP = g.Key.MaSP,
                               TenSP = g.Key.TenSP,
                               SoLuong = g.Sum(x => x.SoLuong),
                               DoanhThu = g.Sum(x => x.SoLuong * x.DonGia)
                           };

                return data.Take(3).ToList();
            }
        }

        // 3. Tổng doanh thu theo tháng (Đã nâng cấp)
        public List<BaoCaoDoanhThu> LayDoanhThuTheoThang()
        {
            using (var db = new QLBanHangDbContext())
            {
                // Bước 1: Lấy dữ liệu thô (Join bảng Hóa Đơn và Chi Tiết để có tiền)
                var query = from hd in db.HoaDons
                            join ct in db.ChiTietHoaDons on hd.MaHD equals ct.MaHD
                            where hd.NgayLap.HasValue
                            select new
                            {
                                MaHD = hd.MaHD,
                                NgayLap = hd.NgayLap.Value,
                                // Tính tiền từng dòng chi tiết (ép kiểu decimal để tránh tràn số)
                                ThanhTien = (decimal)ct.SoLuong * ct.DonGia
                            };

                // Lấy về RAM để xử lý GroupBy (LINQ to Objects)
                var data = query.ToList();

                // Bước 2: Gom nhóm theo Tháng/Năm
                var result = data
                             .GroupBy(x => new { Month = x.NgayLap.Month, Year = x.NgayLap.Year })
                             .OrderByDescending(g => g.Key.Year)
                             .ThenByDescending(g => g.Key.Month)
                             .Select(g => new BaoCaoDoanhThu
                             {
                                 ThoiGian = string.Format("Tháng {0}/{1}", g.Key.Month, g.Key.Year),

                                 // Đếm số hóa đơn (Phải dùng Distinct vì 1 hóa đơn có nhiều chi tiết)
                                 SoLuongDon = g.Select(x => x.MaHD).Distinct().Count(),

                                 // Tính tổng tiền
                                 TongDoanhThu = g.Sum(x => x.ThanhTien)
                             })
                             .ToList();

                return result;
            }
        }
    }
}