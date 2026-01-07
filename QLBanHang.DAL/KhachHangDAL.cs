using System.Collections.Generic;
using System.Linq;
using QLBanHang.DTO;

namespace QLBanHang.DAL
{
    public class KhachHangDAL
    {
        public List<KhachHangDTO> LayDanhSachKhachHang()
        {
            using (var db = new QLBanHangDbContext())
            {
                return db.KhachHangs.Select(kh => new KhachHangDTO
                {
                    MaKH = kh.MaKH,
                    TenKH = kh.TenKH
                }).ToList();
            }
        }
    }
}