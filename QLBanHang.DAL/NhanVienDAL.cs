using System.Collections.Generic;
using System.Linq;
using QLBanHang.DTO;


namespace QLBanHang.DAL
{
    public class NhanVienDAL
    {
        public List<NhanVienDTO> LayDanhSachNhanVien()
        {
            using (var db = new QLBanHangDbContext())
            {
                return db.NhanViens.Select(nv => new NhanVienDTO
                {
                    MaNV = nv.MaNV,
                    TenNV = nv.TenNV
                }).ToList();
            }
        }
    }
}