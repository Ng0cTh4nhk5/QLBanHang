using QLBanHang.BUS; // Nhớ using BUS
using System;
using System.Windows.Forms;

namespace QLBanHang.GUI
{
    public partial class frmThongKe : Form
    {
        // Gọi BUS thống kê (Bạn nhớ tạo class ThongKeBUS và ThongKeDAL như tin nhắn trước nhé)
        ThongKeBUS bus = new ThongKeBUS();

        public frmThongKe()
        {
            InitializeComponent();
        }

        private void frmThongKe_Load(object sender, EventArgs e)
        {
            // Cấu hình tự động tạo cột cho CẢ 3 lưới dữ liệu
            dgvHoaDon.AutoGenerateColumns = true;
            dgvTopSanPham.AutoGenerateColumns = true;
            dgvDoanhThu.AutoGenerateColumns = true;

            // Set ngày mặc định cho Tab 1
            dtpTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDenNgay.Value = DateTime.Now;

            // Load dữ liệu cho Tab 2 (Top 3) và Tab 3 (Doanh thu) ngay khi mở form
            TaiDuLieuTop3();
            TaiDuLieuDoanhThuThang();
        }

        // --- TAB 1: DANH SÁCH HÓA ĐƠN THEO NGÀY ---
        private void btnThongKeHD_Click(object sender, EventArgs e)
        {
            dgvHoaDon.DataSource = bus.LayDsHoaDonTheoNgay(dtpTuNgay.Value, dtpDenNgay.Value);
        }

        // --- TAB 2: TOP 3 SẢN PHẨM BÁN CHẠY ---
        private void TaiDuLieuTop3()
        {
            dgvTopSanPham.DataSource = bus.LayTop3SanPham();
            // Tự động chỉnh độ rộng cột
            dgvTopSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // --- TAB 3: DOANH THU THEO THÁNG ---
        private void TaiDuLieuDoanhThuThang()
        {
            dgvDoanhThu.DataSource = bus.LayDoanhThuTheoThang();
            dgvDoanhThu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Format cột tiền (nếu cột tên là 'Tổng_Doanh_Thu')
            if (dgvDoanhThu.Columns["Tổng_Doanh_Thu"] != null)
                dgvDoanhThu.Columns["Tổng_Doanh_Thu"].DefaultCellStyle.Format = "N0";
        }
    }
}