using QLBanHang.BUS;
using QLBanHang.DTO;
using System;
using System.Data; // Cần thiết để dùng DataTable/List
using System.Drawing; // Dùng cho Color
using System.Linq;
using System.Windows.Forms;

namespace QLBanHang.GUI
{
    public partial class frmSanPham : Form
    {
        // Sử dụng readonly và underscore theo chuẩn
        private readonly SanPhamBUS _sanPhamBUS;

        public frmSanPham()
        {
            InitializeComponent();
            _sanPhamBUS = new SanPhamBUS();
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            TrangTriGiaoDien();
            TaiDuLieu();
        }

        private void TaiDuLieu()
        {
            dgvSanPham.DataSource = _sanPhamBUS.LayDanhSachSanPham();

            // Format cột hiển thị tiền tệ ngay sau khi load
            if (dgvSanPham.Columns["DonGia"] != null)
            {
                dgvSanPham.Columns["DonGia"].DefaultCellStyle.Format = "N0"; // 10,000
            }
        }

        private void ResetForm()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtDonGia.Text = "0";
            txtSoLuong.Text = "0";
            txtTimKiem.Clear();
            chkTrangThai.Checked = true;
            txtTenSP.Focus(); // Focus vào ô tên để nhập liệu nhanh
        }

        // --- CÁC NÚT CHỨC NĂNG ---

        private void btnThem_Click(object sender, EventArgs e)
        {
            // 1. Validate dữ liệu an toàn (Dùng TryParse)
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSP.Focus();
                return;
            }

            if (!decimal.TryParse(txtDonGia.Text, out decimal donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá phải là số và không âm!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong < 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string tenSP = txtTenSP.Text.Trim();

                // 2. Kiểm tra tồn tại
                var spTonTai = _sanPhamBUS.LaySanPhamTheoTen(tenSP);

                if (spTonTai == null)
                {
                    // Case A: Thêm mới
                    var spMoi = new SanPhamDTO
                    {
                        TenSP = tenSP,
                        DonGia = donGia,
                        SoLuong = soLuong,
                        TrangThai = chkTrangThai.Checked
                    };

                    if (_sanPhamBUS.ThemSanPham(spMoi))
                    {
                        MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        TaiDuLieu();
                        ResetForm();
                    }
                }
                else
                {
                    // Case B: Đã tồn tại -> Xử lý cộng dồn
                    XuLyCongDon(spTonTai, donGia, soLuong);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void XuLyCongDon(SanPhamDTO spTonTai, decimal giaMoi, int slThem)
        {
            spTonTai.SoLuong += slThem;

            // Logic hỏi giá
            if (spTonTai.DonGia != giaMoi)
            {
                string msg = $"Sản phẩm '{spTonTai.TenSP}' đã có giá {spTonTai.DonGia:N0}.\n" +
                             $"Bạn nhập giá mới {giaMoi:N0}.\n\n" +
                             "Chọn YES để cập nhật giá MỚI.\n" +
                             "Chọn NO để giữ giá CŨ (chỉ cộng số lượng).";

                if (MessageBox.Show(msg, "Xác nhận giá", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    spTonTai.DonGia = giaMoi;
                }
            }

            if (_sanPhamBUS.SuaSanPham(spTonTai))
            {
                MessageBox.Show($"Đã cập nhật kho! Tổng số lượng: {spTonTai.SoLuong}", "Thành công");
                TaiDuLieu();
                ResetForm();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSP.Text) || !int.TryParse(txtMaSP.Text, out int maSP))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Cảnh báo");
                return;
            }

            // Validate lại số liệu
            if (!decimal.TryParse(txtDonGia.Text, out decimal donGia) || donGia < 0) return;
            if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong < 0) return;

            var sp = new SanPhamDTO
            {
                MaSP = maSP,
                TenSP = txtTenSP.Text.Trim(),
                DonGia = donGia,
                SoLuong = soLuong,
                TrangThai = chkTrangThai.Checked
            };

            if (_sanPhamBUS.SuaSanPham(sp))
            {
                MessageBox.Show("Sửa thành công!", "Thông báo");
                TaiDuLieu();
                ResetForm();
            }
            else
            {
                MessageBox.Show("Sửa thất bại!", "Lỗi");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSP.Text) || !int.TryParse(txtMaSP.Text, out int maSP))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Cảnh báo");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (_sanPhamBUS.XoaSanPham(maSP))
                {
                    MessageBox.Show("Đã xóa thành công!");
                    TaiDuLieu();
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại (Có thể sản phẩm đã có hóa đơn)!", "Lỗi");
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            dgvSanPham.DataSource = _sanPhamBUS.TimKiemSanPham(txtTimKiem.Text.Trim());
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ResetForm();
            TaiDuLieu();
        }

        private void chkLocTonKho_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLocTonKho.Checked)
            {
                var danhSach = _sanPhamBUS.LayDanhSachSanPham();
                dgvSanPham.DataSource = danhSach.Where(x => x.SoLuong > 0).ToList();
            }
            else
            {
                TaiDuLieu();
            }
        }

        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Bảo vệ tránh click vào Header (RowIndex = -1) gây lỗi
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvSanPham.Rows[e.RowIndex];

            txtMaSP.Text = row.Cells["MaSP"].Value?.ToString();
            txtTenSP.Text = row.Cells["TenSP"].Value?.ToString();

            // Format hiển thị lại cho đẹp khi click ngược lại TextBox
            if (decimal.TryParse(row.Cells["DonGia"].Value?.ToString(), out decimal gia))
                txtDonGia.Text = gia.ToString("N0"); // 10,000

            txtSoLuong.Text = row.Cells["SoLuong"].Value?.ToString();

            if (row.Cells["TrangThai"].Value != null)
                chkTrangThai.Checked = (bool)row.Cells["TrangThai"].Value;
        }

        // --- TRANG TRÍ ---
        private void TrangTriGiaoDien()
        {
            this.BackColor = Color.WhiteSmoke;
            groupBox1.BackColor = Color.White;
            groupBox1.ForeColor = Color.DarkBlue;

            // GridView
            dgvSanPham.BackgroundColor = Color.White;
            dgvSanPham.BorderStyle = BorderStyle.None;
            dgvSanPham.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgvSanPham.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgvSanPham.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;

            dgvSanPham.EnableHeadersVisualStyles = false;
            dgvSanPham.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvSanPham.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dgvSanPham.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSanPham.ColumnHeadersHeight = 35;

            // Buttons
            StyleButton(btnThem, Color.ForestGreen);
            StyleButton(btnSua, Color.Goldenrod);
            StyleButton(btnXoa, Color.Crimson);
            StyleButton(btnLamMoi, Color.SteelBlue);
            StyleButton(btnTimKiem, Color.DimGray);
        }

        private void StyleButton(Button btn, Color color)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
        }
    }
}