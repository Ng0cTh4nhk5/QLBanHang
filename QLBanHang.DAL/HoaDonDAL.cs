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
            // Khởi tạo context
            using (var db = new QLBanHangContextDataContext())
            {
                // Mở kết nối
                if (db.Connection.State == System.Data.ConnectionState.Closed)
                    db.Connection.Open();

                // Bắt đầu Transaction (Giao dịch) để đảm bảo toàn vẹn dữ liệu
                // Nghĩa là: Lưu cả 2 bảng thành công mới tính là xong. 1 bảng lỗi là hủy hết.
                using (var transaction = db.Connection.BeginTransaction())
                {
                    db.Transaction = transaction;

                    try
                    {
                        // BƯỚC 1: Lưu bảng Hóa Đơn (Header)
                        HoaDon hdEntity = new HoaDon();
                        hdEntity.NgayLap = hdDTO.NgayLap;
                        hdEntity.MaKH = hdDTO.MaKH;
                        hdEntity.MaNV = hdDTO.MaNV;
             

                        db.HoaDons.InsertOnSubmit(hdEntity);
                        db.SubmitChanges(); // Chạy dòng này để SQL sinh ra MaHD

                        // Lấy MaHD vừa sinh ra để gán cho chi tiết
                        int maHDMoi = hdEntity.MaHD;

                        // BƯỚC 2: Lưu bảng Chi Tiết (Details)
                        foreach (var item in listChiTiet)
                        {
                            ChiTietHoaDon ctEntity = new ChiTietHoaDon();
                            ctEntity.MaHD = maHDMoi; // Quan trọng: Gán mã HĐ vừa tạo
                            ctEntity.MaSP = item.MaSP;
                            ctEntity.SoLuong = item.SoLuong;
                            ctEntity.DonGia = item.DonGia;
                        

                            db.ChiTietHoaDons.InsertOnSubmit(ctEntity);
                        }

                        // Lưu toàn bộ chi tiết
                        db.SubmitChanges();

                        // Nếu code chạy đến đây nghĩa là ngon lành -> Xác nhận giao dịch
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Nếu có bất kỳ lỗi gì -> Quay ngược thời gian, hủy hết những gì đã lưu
                        transaction.Rollback();
                        // (Tùy chọn) Ném lỗi ra để debug: throw ex;
                        return false;
                    }
                }
            }
        }
    }
}