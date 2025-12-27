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
        // Hàm lưu hóa đơn và chi tiết hóa đơn (Transaction)
        public bool LuuHoaDon(HoaDonDTO hdDTO, List<ChiTietHoaDonDTO> listCT)
        {
            using (QLBanHangContextDataContext db = new QLBanHangContextDataContext())
            {
                try
                {
                    // 1. Tạo và lưu Hóa Đơn (Master) trước
                    HoaDon hd = new HoaDon
                    {
                        NgayLap = hdDTO.NgayLap,
                        MaNV = hdDTO.MaNV,
                        MaKH = hdDTO.MaKH
                    };
                    db.HoaDons.InsertOnSubmit(hd);
                    db.SubmitChanges(); // Lưu để sinh ra MaHD tự động

                    // 2. Lấy MaHD vừa sinh ra gán cho danh sách Chi tiết
                    foreach (var item in listCT)
                    {
                        ChiTietHoaDon ct = new ChiTietHoaDon
                        {
                            MaHD = hd.MaHD, // Lấy ID vừa insert xong
                            MaSP = item.MaSP,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia
                        };
                        db.ChiTietHoaDons.InsertOnSubmit(ct);

                        // Cập nhật trừ tồn kho (Logic nâng cao nếu cần)
                        var sp = db.SanPhams.SingleOrDefault(s => s.MaSP == item.MaSP);
                        if (sp != null) sp.SoLuong -= item.SoLuong;
                    }

                    // 3. Lưu toàn bộ chi tiết
                    db.SubmitChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
