using QLBanHang.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBanHang.DAL
{
    public class HoaDonDAL
    {
        public bool LuuHoaDon(HoaDonDTO hdDTO, List<ChiTietHoaDonDTO> listChiTiet)
        {
            // Khởi tạo context (Tự động lấy chuỗi kết nối từ App.config nếu đã cấu hình đúng)
            using (var db = new QLBanHangContextDataContext())
            {
                // Mở kết nối thủ công để gắn vào Transaction
                if (db.Connection.State == System.Data.ConnectionState.Closed)
                    db.Connection.Open();

                // Bắt đầu Transaction
                using (var transaction = db.Connection.BeginTransaction())
                {
                    // Gán transaction cho context để mọi lệnh LINQ đều nằm trong giao dịch này
                    db.Transaction = transaction;

                    try
                    {
                        // --- BƯỚC 1: LƯU HÓA ĐƠN (HEADER) ---
                        HoaDon hdEntity = new HoaDon();
                        hdEntity.NgayLap = hdDTO.NgayLap;
                        hdEntity.MaKH = hdDTO.MaKH;
                        hdEntity.MaNV = hdDTO.MaNV;
                        // hdEntity.TongTien = hdDTO.TongTien; // Nếu trong DB bạn có cột này thì mở ra

                        db.HoaDons.InsertOnSubmit(hdEntity);
                        db.SubmitChanges(); // Đẩy xuống DB để lấy MaHD (nhưng chưa Commit hẳn)

                        int maHDMoi = hdEntity.MaHD; // Lấy ID tự tăng

                        // --- BƯỚC 2: LƯU CHI TIẾT & TRỪ KHO ---
                        foreach (var item in listChiTiet)
                        {
                            // 2.1. Tạo chi tiết hóa đơn
                            ChiTietHoaDon ctEntity = new ChiTietHoaDon();
                            ctEntity.MaHD = maHDMoi;
                            ctEntity.MaSP = item.MaSP;
                            ctEntity.SoLuong = item.SoLuong;
                            ctEntity.DonGia = item.DonGia;

                            db.ChiTietHoaDons.InsertOnSubmit(ctEntity);

                            // 2.2. XỬ LÝ TRỪ TỒN KHO (Logic mới thêm)
                            // Tìm sản phẩm tương ứng trong context hiện tại
                            var sanPham = db.SanPhams.SingleOrDefault(sp => sp.MaSP == item.MaSP);

                            if (sanPham != null)
                            {
                                // (Tùy chọn) Kiểm tra tồn kho lần cuối cho chắc chắn
                                if (sanPham.SoLuong < item.SoLuong)
                                {
                                    throw new Exception($"Sản phẩm mã {item.MaSP} không đủ hàng để bán!");
                                    // Ném lỗi này thì code sẽ nhảy xuống catch -> Rollback ngay lập tức
                                }

                                // Trừ số lượng
                                sanPham.SoLuong = sanPham.SoLuong - item.SoLuong;

                                // Lưu ý: Với LINQ, bạn không cần gọi lệnh Update. 
                                // Chỉ cần sửa thuộc tính, LINQ tự biết để sinh lệnh UPDATE SanPham...
                            }
                        }

                        // --- BƯỚC 3: GỬI TẤT CẢ XUỐNG DB ---
                        // Lúc này nó sẽ chạy một loạt lệnh INSERT ChiTiet và UPDATE SanPham
                        db.SubmitChanges();

                        // Nếu chạy êm xuôi đến đây -> Chốt đơn!
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Có biến -> Hủy hết, trả lại tiền, trả lại kho
                        transaction.Rollback();

                        // Bạn có thể log lỗi ra file text ở đây nếu muốn
                        // Console.WriteLine(ex.Message);
                        return false;
                    }
                }
            }
        }

        public dsHoaDon LayDuLieuInHoaDon(int maHD)
        {
            dsHoaDon ds = new dsHoaDon();

            using (var db = new QLBanHangContextDataContext())
            {
                // 1. Lấy thông tin Header (Hóa đơn + Khách + Nhân viên)
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
                    // Đổ vào bảng Header của Dataset
                    var rowHead = ds.dtHeader.NewdtHeaderRow();
                    rowHead.MaHoaDon = hdInfo.MaHD;
                    rowHead.NgayLap = hdInfo.NgayLap.Value;
                    rowHead.TenKhachHang = hdInfo.TenKH;
                    rowHead.DiaChi = hdInfo.DiaChi;
                    rowHead.SoDienThoai = hdInfo.DienThoai;
                    rowHead.TenNhanVien = hdInfo.TenNV;

                    // Tạm thời để tổng tiền = 0, tý tính sau hoặc lấy từ DB
                    rowHead.TongTien = 0;
                    rowHead.TongTienChu = "";

                    ds.dtHeader.AdddtHeaderRow(rowHead);
                }

                // 2. Lấy thông tin Detail (Chi tiết sản phẩm)
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

                    // Cộng dồn tổng tiền
                    tongTien += (decimal)item.ThanhTien;
                }

                // Cập nhật lại tổng tiền cho Header (nếu tìm thấy header)
                if (ds.dtHeader.Rows.Count > 0)
                {
                    ds.dtHeader[0].TongTien = tongTien;
                    // Gọi hàm đọc số thành chữ ở đây nếu muốn, hoặc xử lý ở BUS
                }
            }

            return ds;
        }
    }
}