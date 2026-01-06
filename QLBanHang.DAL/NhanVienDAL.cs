using System;
using System.Collections.Generic;
using System.Linq;
using QLBanHang.DTO;
using QLBanHang.DAL.Entities; // Quan trọng: Để nhận diện class NhanVien mới

namespace QLBanHang.DAL
{
    public class NhanVienDAL
    {
        // 1. Lấy danh sách nhân viên
        public List<NhanVienDTO> LayDanhSachNhanVien()
        {
            using (var db = new QLBanHangDbContext())
            {
                // Mapping từ Entity sang DTO
                return db.NhanViens.Select(nv => new NhanVienDTO
                {
                    MaNV = nv.MaNV,
                    TenNV = nv.TenNV
                }).ToList();
            }
        }

        
    }
}