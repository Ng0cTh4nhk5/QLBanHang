using System;
using System.Collections.Generic;
using System.Linq;
using QLBanHang.DTO;
using QLBanHang.DAL.Entities; // Namespace chứa Entity KhachHang

namespace QLBanHang.DAL
{
    public class KhachHangDAL
    {
        private QLBanHangDbContext db;

        public KhachHangDAL()
        {
            db = new QLBanHangDbContext();
        }

        // 1. Lấy danh sách
        public List<KhachHangDTO> LayDanhSachKhachHang()
        {
            using (var context = new QLBanHangDbContext())
            {
                return context.KhachHangs.Select(kh => new KhachHangDTO
                {
                    MaKH = kh.MaKH,
                    TenKH = kh.TenKH,
                }).ToList();
            }
        }

       
    }
}