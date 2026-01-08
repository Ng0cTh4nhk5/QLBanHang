using System.Collections.Generic;
using System.Linq;
using QLBanHang.DTO;
using QLBanHang.DAL.Entities; 

namespace QLBanHang.DAL
{
    public class NhanVienDAL
    {
        // 1. Lấy danh sách 
        public List<NhanVienDTO> LayDanhSachNhanVien()
        {
            using (var db = new QLBanHangDbContext())
            {
                // Lưu ý: Map đầy đủ các trường cần thiết để hiển thị
                return db.NhanViens.Select(nv => new NhanVienDTO
                {
                    MaNV = nv.MaNV,
                    TenNV = nv.TenNV,
                    ChucVu = nv.ChucVu,     // Map thêm
                    DienThoai = nv.DienThoai // Map thêm
                }).ToList();
            }
        }

        // 2. Thêm nhân viên
        public bool ThemNhanVien(NhanVienDTO nvDTO)
        {
            using (var db = new QLBanHangDbContext())
            {
                // Tạo Entity từ DTO
                var nv = new NhanVien()
                {
                    // MaNV là Identity (tự tăng), không cần gán
                    TenNV = nvDTO.TenNV,
                    ChucVu = nvDTO.ChucVu,
                    DienThoai = nvDTO.DienThoai
                };

                db.NhanViens.Add(nv);

                // SaveChanges trả về số dòng bị ảnh hưởng. > 0 nghĩa là thành công
                return db.SaveChanges() > 0;
            }
        }

        // 3. Sửa nhân viên
        public bool SuaNhanVien(NhanVienDTO nvDTO)
        {
            using (var db = new QLBanHangDbContext())
            {
                // Tìm nhân viên cần sửa theo Mã
                var nv = db.NhanViens.SingleOrDefault(x => x.MaNV == nvDTO.MaNV);

                if (nv == null) return false; // Không tìm thấy để sửa

                // Cập nhật thông tin mới
                nv.TenNV = nvDTO.TenNV;
                nv.ChucVu = nvDTO.ChucVu;
                nv.DienThoai = nvDTO.DienThoai;

                return db.SaveChanges() > 0;
            }
        }

        // 4. Xóa nhân viên
        public bool XoaNhanVien(int maNV)
        {
            using (var db = new QLBanHangDbContext())
            {
                // Tìm nhân viên cần xóa
                var nv = db.NhanViens.SingleOrDefault(x => x.MaNV == maNV);

                if (nv == null) return false; // Không tìm thấy

                db.NhanViens.Remove(nv);
                return db.SaveChanges() > 0;
            }
        }
    }
}