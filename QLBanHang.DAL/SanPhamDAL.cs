using System;
using System.Collections.Generic;
using System.Linq;
using QLBanHang.DTO;
using QLBanHang.DAL.Entities;

namespace QLBanHang.DAL
{
    public class SanPhamDAL
    {
        /// <summary>
        /// Lấy danh sách sản phẩm (Mapping Entity -> DTO)
        /// </summary>
        public List<SanPhamDTO> LayDanhSachSanPham()
        {
            using (var db = new QLBanHangDbContext())
            {
                return db.SanPhams.Select(sp => new SanPhamDTO
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    DonGia = sp.DonGia, 
                    SoLuong = sp.SoLuong,
                    TrangThai = sp.TrangThai
                }).ToList();
            }
        }

        public bool ThemSanPham(SanPhamDTO spDto)
        {
            using (var db = new QLBanHangDbContext())
            {
                try
                {
                    var spEntity = new SanPham
                    {
                        TenSP = spDto.TenSP,
                        DonGia = spDto.DonGia,
                        SoLuong = spDto.SoLuong,
                        TrangThai = spDto.TrangThai
                    };

                    db.SanPhams.Add(spEntity);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool SuaSanPham(SanPhamDTO spDto)
        {
            using (var db = new QLBanHangDbContext())
            {
                try
                {
                    var spEntity = db.SanPhams.Find(spDto.MaSP);
                    if (spEntity == null) return false;

                    spEntity.TenSP = spDto.TenSP;
                    spEntity.DonGia = spDto.DonGia;
                    spEntity.SoLuong = spDto.SoLuong;
                    spEntity.TrangThai = spDto.TrangThai;

                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool XoaSanPham(int maSP)
        {
            using (var db = new QLBanHangDbContext())
            {
                try
                {
                    var spEntity = db.SanPhams.Find(maSP);
                    if (spEntity == null) return false;

                    db.SanPhams.Remove(spEntity);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public SanPham LaySanPhamTheoTen(string tenSP)
        {
            using (var db = new QLBanHangDbContext())
            {
                return db.SanPhams.FirstOrDefault(sp => sp.TenSP == tenSP);
            }
        }
    }
}