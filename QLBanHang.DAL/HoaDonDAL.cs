using QLBanHang.DTO;
using QLBanHang.DAL.Entities; // Import namespace chứa Entity
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; // Cần thiết cho các thao tác EF

namespace QLBanHang.DAL
{
    public class HoaDonDAL
    {
        public bool LuuHoaDon(HoaDonDTO hdDTO, List<ChiTietHoaDonDTO> listChiTiet)
        {
            using (var db = new QLBanHangDbContext())
            {
                // Trong EF6, BeginTransaction được gọi từ db.Database
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // --- BƯỚC 1: LƯU HÓA ĐƠN (HEADER) ---
                        HoaDon hdEntity = new HoaDon();
                        hdEntity.NgayLap = hdDTO.NgayLap;
                        hdEntity.MaKH = hdDTO.MaKH;
                        hdEntity.MaNV = hdDTO.MaNV;

                        db.HoaDons.Add(hdEntity); // Thay InsertOnSubmit bằng Add
                        db.SaveChanges(); // Lưu để EF tự động lấy ID (MaHD) về hdEntity

                        int maHDMoi = hdEntity.MaHD;

                        // --- BƯỚC 2: LƯU CHI TIẾT & TRỪ KHO ---
                        foreach (var item in listChiTiet)
                        {
                            // 2.1. Tạo chi tiết hóa đơn
                            ChiTietHoaDon ctEntity = new ChiTietHoaDon();
                            ctEntity.MaHD = maHDMoi;
                            ctEntity.MaSP = item.MaSP;
                            ctEntity.SoLuong = item.SoLuong;
                            ctEntity.DonGia = item.DonGia;

                            db.ChiTietHoaDons.Add(ctEntity);

                            // 2.2. XỬ LÝ TRỪ TỒN KHO
                            // Tìm sản phẩm 
                            var sanPham = db.SanPhams.SingleOrDefault(sp => sp.MaSP == item.MaSP);

                            if (sanPham != null)
                            {
                                if (sanPham.SoLuong < item.SoLuong)
                                {
                                    throw new Exception($"Sản phẩm mã {item.MaSP} không đủ hàng để bán!");
                                }

                                // Trừ số lượng (EF tự động theo dõi thay đổi này)
                                sanPham.SoLuong = sanPham.SoLuong - item.SoLuong;
                            }
                        }

                        // --- BƯỚC 3: GỬI TẤT CẢ XUỐNG DB ---
                        db.SaveChanges(); // Thực thi các lệnh Insert ChiTiet và Update SanPham

                        // Commit transaction
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction nếu có lỗi
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public dsHoaDon LayDuLieuInHoaDon(int maHD)
        {
            // Giả định dsHoaDon là một Typed Dataset đã có sẵn trong project của bạn
            dsHoaDon ds = new dsHoaDon();

            using (var db = new QLBanHangDbContext())
            {
                // 1. Lấy thông tin Header
                // Query giống hệt cũ, chỉ đổi db source
                var hdInfo = (from hd in db.HoaDons
                              join kh in db.KhachHangs on hd.MaKH equals kh.MaKH
                              join nv in db.NhanViens on hd.MaNV equals nv.MaNV
                              where hd.MaHD == maHD
                              select new
                              {
                                  hd.MaHD,
                                  hd.NgayLap,
                                  kh.TenKH,
                                  kh.DiaChi,
                                  kh.DienThoai,
                                  nv.TenNV
                              }).FirstOrDefault();

                if (hdInfo != null)
                {
                    var rowHead = ds.dtHeader.NewdtHeaderRow();
                    rowHead.MaHoaDon = hdInfo.MaHD;
                    // Xử lý Nullable DateTime an toàn
                    rowHead.NgayLap = hdInfo.NgayLap ?? DateTime.Now;
                    rowHead.TenKhachHang = hdInfo.TenKH;
                    rowHead.DiaChi = hdInfo.DiaChi;
                    rowHead.SoDienThoai = hdInfo.DienThoai;
                    rowHead.TenNhanVien = hdInfo.TenNV;

                    rowHead.TongTien = 0;
                    rowHead.TongTienChu = "";

                    ds.dtHeader.AdddtHeaderRow(rowHead);
                }

                // 2. Lấy thông tin Detail
                var listChiTiet = (from ct in db.ChiTietHoaDons
                                   join sp in db.SanPhams on ct.MaSP equals sp.MaSP
                                   where ct.MaHD == maHD
                                   select new
                                   {
                                       sp.TenSP,
                                       ct.SoLuong,
                                       ct.DonGia,
                                       ThanhTien = ct.SoLuong * ct.DonGia
                                   }).ToList();

                int stt = 1;
                decimal tongTien = 0;

                foreach (var item in listChiTiet)
                {
                    var rowDetail = ds.dtDetail.NewdtDetailRow();
                    rowDetail.MaHoaDon = maHD;
                    rowDetail.STT = stt++;
                    rowDetail.TenSP = item.TenSP;

                    rowDetail.SoLuong = (int)item.SoLuong;
                    rowDetail.DonGia = (decimal)item.DonGia;
                    rowDetail.ThanhTien = (decimal)item.ThanhTien;

                    ds.dtDetail.AdddtDetailRow(rowDetail);

                    tongTien += (decimal)item.ThanhTien;
                }

                if (ds.dtHeader.Rows.Count > 0)
                {
                    ds.dtHeader[0].TongTien = tongTien;
                }
            }

            return ds;
        }
    }
}