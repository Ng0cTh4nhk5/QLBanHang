using System;
using System.Collections.Generic;
using System.Linq;

namespace QLBanHang.DAL
{
    public class ThongKeDAL
    {
        // 1. Danh sách hóa đơn theo khoảng ngày
        public object LayDsHoaDonTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            using (var db = new QLBanHangContextDataContext())
            {
                // Chọn các cột cần hiển thị
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

        // 2. Top 3 sản phẩm bán chạy nhất (Tính theo tổng số lượng bán)
        public object LayTop3SanPhamBanChay()
        {
            using (var db = new QLBanHangContextDataContext())
            {
                var data = from ct in db.ChiTietHoaDons
                           join sp in db.SanPhams on ct.MaSP equals sp.MaSP
                           // Gom nhóm theo Mã SP và Tên SP
                           group ct by new { sp.MaSP, sp.TenSP } into g
                           // Sắp xếp giảm dần theo tổng số lượng
                           orderby g.Sum(x => x.SoLuong) descending
                           select new
                           {
                               Mã_SP = g.Key.MaSP,
                               Tên_Sản_Phẩm = g.Key.TenSP,
                               Tổng_Số_Lượng_Bán = g.Sum(x => x.SoLuong),
                               Tổng_Doanh_Thu = g.Sum(x => x.SoLuong * x.DonGia)
                           };

                // Lấy 3 dòng đầu tiên
                return data.Take(3).ToList();
            }
        }

        // 3. Tổng doanh thu theo tháng (Của năm hiện tại hoặc tất cả)
        public object LayDoanhThuTheoThang()
        {
            using (var db = new QLBanHangContextDataContext())
            {
                var data = from hd in db.HoaDons
                           where hd.NgayLap != null
                           // Gom nhóm theo Tháng và Năm
                           group hd by new { Thang = hd.NgayLap.Value.Month, Nam = hd.NgayLap.Value.Year } into g
                           orderby g.Key.Nam descending, g.Key.Thang descending
                           select new
                           {
                               Tháng = "Tháng " + g.Key.Thang + "/" + g.Key.Nam,
                               Số_Lượng_Đơn = g.Count(),
                           };

                return data.ToList();
            }
        }
    }
}