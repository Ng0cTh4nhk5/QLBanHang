using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace QLBanHang.DAL
{
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
    }

    public class ThongKeDAL
    {
        public object LayDsHoaDonTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            using (var db = new QLBanHangDbContext())
            {
                // Dùng DbFunctions.TruncateTime để so sánh ngày mà không quan tâm giờ phút giây
                return (from hd in db.HoaDons
                        join kh in db.KhachHangs on hd.MaKH equals kh.MaKH
                        join nv in db.NhanViens on hd.MaNV equals nv.MaNV
                        where DbFunctions.TruncateTime(hd.NgayLap) >= DbFunctions.TruncateTime(tuNgay)
                           && DbFunctions.TruncateTime(hd.NgayLap) <= DbFunctions.TruncateTime(denNgay)
                        select new
                        {
                            hd.MaHD,
                            hd.NgayLap,
                            kh.TenKH,
                            nv.TenNV
                        }).ToList();
            }
        }

        public List<BaoCaoTopSanPham> LayTop3SanPhamBanChay()
        {
            using (var db = new QLBanHangDbContext())
            {
                return (from ct in db.ChiTietHoaDons
                        join sp in db.SanPhams on ct.MaSP equals sp.MaSP
                        group ct by new { sp.MaSP, sp.TenSP } into g
                        orderby g.Sum(x => x.SoLuong) descending
                        select new BaoCaoTopSanPham
                        {
                            MaSP = g.Key.MaSP,
                            TenSP = g.Key.TenSP,
                            SoLuong = g.Sum(x => x.SoLuong),
                            DoanhThu = g.Sum(x => x.SoLuong * x.DonGia)
                        }).Take(3).ToList();
            }
        }

        public List<BaoCaoDoanhThu> LayDoanhThuTheoThang()
        {
            using (var db = new QLBanHangDbContext())
            {
                // 1. Lấy dữ liệu cần thiết lên bộ nhớ (Vì LINQ to Entities hạn chế hàm Format DateTime)
                var rawData = (from hd in db.HoaDons
                               join ct in db.ChiTietHoaDons on hd.MaHD equals ct.MaHD
                               select new
                               {
                                   hd.MaHD,
                                   hd.NgayLap,
                                   ThanhTien = ct.SoLuong * ct.DonGia
                               }).ToList();

                // 2. Xử lý GroupBy trên Memory
                return rawData
                       .GroupBy(x => new { x.NgayLap.Month, x.NgayLap.Year })
                       .OrderByDescending(g => g.Key.Year)
                       .ThenByDescending(g => g.Key.Month)
                       .Select(g => new BaoCaoDoanhThu
                       {
                           ThoiGian = $"Tháng {g.Key.Month}/{g.Key.Year}",
                           SoLuongDon = g.Select(x => x.MaHD).Distinct().Count(),
                           TongDoanhThu = g.Sum(x => x.ThanhTien)
                       }).ToList();
            }
        }
    }
}