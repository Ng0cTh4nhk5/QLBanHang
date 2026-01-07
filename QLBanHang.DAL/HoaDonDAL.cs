using QLBanHang.DTO;
using QLBanHang.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLBanHang.DAL
{
    public class HoaDonDAL
    {
        /// <summary>
        /// Lưu hóa đơn và chi tiết (Transaction: Thành công tất cả hoặc không lưu gì cả)
        /// </summary>
        public bool LuuHoaDon(HoaDonDTO hdDTO, List<ChiTietHoaDonDTO> listChiTiet)
        {
            using (var db = new QLBanHangDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Lưu Header
                        var hdEntity = new HoaDon
                        {
                            NgayLap = hdDTO.NgayLap,
                            MaKH = hdDTO.MaKH,
                            MaNV = hdDTO.MaNV
                        };

                        db.HoaDons.Add(hdEntity);
                        db.SaveChanges(); // Để lấy được MaHD sinh tự động

                        // 2. Lưu Detail & Trừ kho
                        foreach (var item in listChiTiet)
                        {
                            // Thêm chi tiết
                            db.ChiTietHoaDons.Add(new ChiTietHoaDon
                            {
                                MaHD = hdEntity.MaHD,
                                MaSP = item.MaSP,
                                SoLuong = item.SoLuong,
                                DonGia = item.DonGia
                            });

                            // Trừ kho
                            var sanPham = db.SanPhams.Find(item.MaSP);
                            if (sanPham == null || sanPham.SoLuong < item.SoLuong)
                            {
                                throw new Exception("Không đủ hàng trong kho");
                            }
                            sanPham.SoLuong -= item.SoLuong;
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Lấy dữ liệu phục vụ in ấn (Crystal Report / RDLC)
        /// </summary>
        public dsHoaDon LayDuLieuInHoaDon(int maHD)
        {
            var ds = new dsHoaDon(); // Dataset Typed bạn đã định nghĩa

            using (var db = new QLBanHangDbContext())
            {
                // 1. Query Header
                var hdInfo = (from hd in db.HoaDons
                              join kh in db.KhachHangs on hd.MaKH equals kh.MaKH
                              join nv in db.NhanViens on hd.MaNV equals nv.MaNV
                              where hd.MaHD == maHD
                              select new { hd, kh, nv }).FirstOrDefault();

                if (hdInfo != null)
                {
                    var rowHead = ds.dtHeader.NewdtHeaderRow();
                    rowHead.MaHoaDon = hdInfo.hd.MaHD;
                    rowHead.NgayLap = hdInfo.hd.NgayLap; // Đã bỏ nullable
                    rowHead.TenKhachHang = hdInfo.kh.TenKH;
                    rowHead.DiaChi = hdInfo.kh.DiaChi;
                    rowHead.SoDienThoai = hdInfo.kh.DienThoai;
                    rowHead.TenNhanVien = hdInfo.nv.TenNV;

                    // Logic tính tổng sẽ được tính lại khi loop chi tiết
                    rowHead.TongTien = 0;
                    ds.dtHeader.AdddtHeaderRow(rowHead);
                }

                // 2. Query Detail
                var details = (from ct in db.ChiTietHoaDons
                               join sp in db.SanPhams on ct.MaSP equals sp.MaSP
                               where ct.MaHD == maHD
                               select new { sp.TenSP, ct.SoLuong, ct.DonGia }).ToList();

                decimal tongTien = 0;
                int stt = 1;

                foreach (var item in details)
                {
                    var rowDetail = ds.dtDetail.NewdtDetailRow();
                    rowDetail.MaHoaDon = maHD;
                    rowDetail.STT = stt++;
                    rowDetail.TenSP = item.TenSP;
                    rowDetail.SoLuong = item.SoLuong;
                    rowDetail.DonGia = item.DonGia;
                    rowDetail.ThanhTien = item.SoLuong * item.DonGia;

                    ds.dtDetail.AdddtDetailRow(rowDetail);
                    tongTien += rowDetail.ThanhTien;
                }

                // Cập nhật lại tổng tiền header
                if (ds.dtHeader.Rows.Count > 0)
                {
                    ds.dtHeader[0].TongTien = tongTien;
                }
            }
            return ds;
        }
    }
}