using QLBanHang.BUS;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLBanHang.GUI
{
    public partial class frmThongKe : Form
    {
        private readonly ThongKeBUS _thongKeBUS;

        public frmThongKe()
        {
            InitializeComponent();
            _thongKeBUS = new ThongKeBUS();
        }

        private void frmThongKe_Load(object sender, EventArgs e)
        {
            CaiDatMacDinh();
            TaiDuLieuTab2();
            TaiDuLieuTab3();
        }

        private void CaiDatMacDinh()
        {
            // Tự động sinh cột
            dgvHoaDon.AutoGenerateColumns = true;
            dgvTopSanPham.AutoGenerateColumns = true;
            dgvDoanhThu.AutoGenerateColumns = true;

            // Cấu hình DateTimePicker: Từ đầu tháng đến hiện tại
            dtpTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDenNgay.Value = DateTime.Now;

            // AutoSize
            dgvHoaDon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTopSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDoanhThu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // --- TAB 1: DANH SÁCH HÓA ĐƠN ---
        private void btnThongKeHD_Click(object sender, EventArgs e)
        {
            dgvHoaDon.DataSource = _thongKeBUS.LayDsHoaDonTheoNgay(dtpTuNgay.Value, dtpDenNgay.Value);

            // Format cột ngày tháng nếu có
            if (dgvHoaDon.Columns["NgayLap"] != null)
                dgvHoaDon.Columns["NgayLap"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        // --- TAB 2: TOP 3 SẢN PHẨM ---
        private void TaiDuLieuTab2()
        {
            dgvTopSanPham.DataSource = _thongKeBUS.LayTop3SanPham();

            if (dgvTopSanPham.Columns["DoanhThu"] != null)
            {
                dgvTopSanPham.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
                dgvTopSanPham.Columns["DoanhThu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        // --- TAB 3: DOANH THU THEO THÁNG ---
        private void TaiDuLieuTab3()
        {
            dgvDoanhThu.DataSource = null; // Reset
            dgvDoanhThu.DataSource = _thongKeBUS.LayDoanhThuTheoThang();

            // Định dạng cột
            FormatCotDoanhThu();
        }

        private void FormatCotDoanhThu()
        {
            // Kiểm tra null trước khi format để tránh lỗi
            if (dgvDoanhThu.Columns["ThoiGian"] != null)
                dgvDoanhThu.Columns["ThoiGian"].HeaderText = "Thời Gian";

            if (dgvDoanhThu.Columns["SoLuongDon"] != null)
                dgvDoanhThu.Columns["SoLuongDon"].HeaderText = "Số Lượng Đơn";

            if (dgvDoanhThu.Columns["TongDoanhThu"] != null)
            {
                dgvDoanhThu.Columns["TongDoanhThu"].HeaderText = "Tổng Doanh Thu";
                dgvDoanhThu.Columns["TongDoanhThu"].DefaultCellStyle.Format = "N0";
                dgvDoanhThu.Columns["TongDoanhThu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
    }
}