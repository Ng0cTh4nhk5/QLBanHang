using System.Collections.Generic;
using System.Linq;
using QLBanHang.DTO;
using QLBanHang.DAL.Entities; 

namespace QLBanHang.DAL
{
    public class KhachHangDAL
    {
        // 1. Lấy danh sách
        public List<KhachHangDTO> LayDanhSachKhachHang()
        {
            using (var db = new QLBanHangDbContext())
            {
                return db.KhachHangs.Select(kh => new KhachHangDTO
                {
                    MaKH = kh.MaKH,
                    TenKH = kh.TenKH,
                    DienThoai = kh.DienThoai, 
                    DiaChi = kh.DiaChi        
                }).ToList();
            }
        }

        // 2. Thêm khách hàng
        public bool ThemKhachHang(KhachHangDTO khDTO)
        {
            using (var db = new QLBanHangDbContext())
            {
                var kh = new KhachHang()
                {
                    TenKH = khDTO.TenKH,
                    DienThoai = khDTO.DienThoai,
                    DiaChi = khDTO.DiaChi
                };

                db.KhachHangs.Add(kh);
                return db.SaveChanges() > 0;
            }
        }

        // 3. Sửa khách hàng
        public bool SuaKhachHang(KhachHangDTO khDTO)
        {
            using (var db = new QLBanHangDbContext())
            {
                var kh = db.KhachHangs.SingleOrDefault(x => x.MaKH == khDTO.MaKH);

                if (kh == null) return false;

                kh.TenKH = khDTO.TenKH;
                kh.DienThoai = khDTO.DienThoai;
                kh.DiaChi = khDTO.DiaChi;

                return db.SaveChanges() > 0;
            }
        }

        // 4. Xóa khách hàng
        public bool XoaKhachHang(int maKH)
        {
            using (var db = new QLBanHangDbContext())
            {
                var kh = db.KhachHangs.SingleOrDefault(x => x.MaKH == maKH);

                if (kh == null) return false;

                db.KhachHangs.Remove(kh);
                return db.SaveChanges() > 0;
            }
        }
    }
}