namespace QLBanHang.DTO
{
    public class ChiTietHoaDonDTO
    {
        public int MaHD { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } // Để hiển thị tên sản phẩm thay vì mã
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        // Tính toán trực tiếp, chỉ đọc (Read-only)
        public decimal ThanhTien => SoLuong * DonGia;
    }
}