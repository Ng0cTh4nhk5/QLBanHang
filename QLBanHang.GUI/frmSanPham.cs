using QLBanHang.BUS;
using QLBanHang.DTO;
using System;
using System.Collections.Generic;
using System.Linq; // Bắt buộc có để dùng LINQ
using System.Windows.Forms;

namespace QLBanHang.GUI
{
    public partial class frmSanPham : Form
    {
        // Khởi tạo BUS để giao tiếp
        SanPhamBUS spBUS = new SanPhamBUS();

        public frmSanPham()
        {
            InitializeComponent();
        }

        // 1. Sự kiện Load Form
        private void frmSanPham_Load(object sender, EventArgs e)
        {
            TrangTriGiaoDien();
            LoadData();
        }

        // Hàm chung để tải dữ liệu
        private void LoadData()
        {
            dgvSanPham.DataSource = spBUS.LayDanhSachSanPham();
        }

        // 2. Nút THÊM
        // 2. Nút THÊM (Đã nâng cấp logic thông minh)
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validate dữ liệu đầu vào cơ bản
                if (string.IsNullOrWhiteSpace(txtTenSP.Text))
                {
                    MessageBox.Show("Tên sản phẩm không được để trống!");
                    return;
                }

                string tenSP = txtTenSP.Text.Trim();
                decimal donGiaNhap = decimal.Parse(txtDonGia.Text);
                int soLuongNhap = int.Parse(txtSoLuong.Text);

                if (donGiaNhap < 0 || soLuongNhap < 0)
                {
                    MessageBox.Show("Đơn giá và số lượng không được âm!");
                    return;
                }

                // 2. Kiểm tra sản phẩm đã tồn tại trong kho chưa?
                SanPhamDTO spTonTai = spBUS.LaySanPhamTheoTen(tenSP);

                if (spTonTai == null)
                {
                    // --- TRƯỜNG HỢP A: SẢN PHẨM MỚI HOÀN TOÀN ---
                    SanPhamDTO spMoi = new SanPhamDTO();
                    spMoi.TenSP = tenSP;
                    spMoi.DonGia = donGiaNhap;
                    spMoi.SoLuong = soLuongNhap;
                    spMoi.TrangThai = chkTrangThai.Checked;

                    if (spBUS.ThemSanPham(spMoi))
                    {
                        MessageBox.Show("Thêm sản phẩm mới thành công!");
                        LoadData();
                        ResetForm();
                    }
                }
                else
                {
                    // --- TRƯỜNG HỢP B: ĐÃ CÓ (TRÙNG TÊN) -> XỬ LÝ CỘNG DỒN ---

                    // a. Cộng dồn số lượng
                    spTonTai.SoLuong += soLuongNhap;

                    // b. Xử lý logic Đơn giá (Hỏi người dùng)
                    if (spTonTai.DonGia != donGiaNhap)
                    {
                        string msg = string.Format(
                            "Sản phẩm '{0}' đã tồn tại!\n" +
                            "- Giá cũ trong kho: {1:N0}\n" +
                            "- Giá bạn vừa nhập: {2:N0}\n\n" +
                            "Bạn có muốn cập nhật theo GIÁ MỚI không?\n" +
                            "(Yes = Lấy giá mới, No = Giữ giá cũ, chỉ cộng số lượng)",
                            spTonTai.TenSP, spTonTai.DonGia, donGiaNhap);

                        DialogResult result = MessageBox.Show(msg, "Xác nhận giá",
                                                              MessageBoxButtons.YesNo,
                                                              MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            spTonTai.DonGia = donGiaNhap; // Cập nhật giá mới
                        }
                        // Nếu No: Giữ nguyên spTonTai.DonGia cũ
                    }

                    // c. Gọi hàm Cập nhật (SuaSanPham)
                    if (spBUS.SuaSanPham(spTonTai))
                    {
                        MessageBox.Show($"Đã nhập thêm hàng! Tổng số lượng hiện tại: {spTonTai.SoLuong}");
                        LoadData();
                        ResetForm();
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi khi cập nhật số lượng!");
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Vui lòng nhập Đơn giá và Số lượng đúng định dạng số!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
            }
        }

        // 3. Nút SỬA (Bổ sung mới)
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSP.Text))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!");
                return;
            }

            try
            {
                SanPhamDTO sp = new SanPhamDTO();
                sp.MaSP = int.Parse(txtMaSP.Text); // Lấy ID cũ
                sp.TenSP = txtTenSP.Text;
                sp.DonGia = decimal.Parse(txtDonGia.Text);
                sp.SoLuong = int.Parse(txtSoLuong.Text);
                sp.TrangThai = chkTrangThai.Checked;

                if (spBUS.SuaSanPham(sp))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadData();
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Sửa thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        // 4. Nút XÓA
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSP.Text)) return;

            if (MessageBox.Show("Bạn muốn xóa SP này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int maSP = int.Parse(txtMaSP.Text);
                if (spBUS.XoaSanPham(maSP))
                {
                    MessageBox.Show("Đã xóa!");
                    LoadData();
                    ResetForm();
                }
            }
        }

        // 5. Nút TÌM KIẾM (Đã fix cho khớp với btnTimKiem)
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text;
            // Gọi hàm tìm kiếm bên BUS (đã viết ở bước trước)
            dgvSanPham.DataSource = spBUS.TimKiemSanPham(tuKhoa);
        }

        // 6. Nút LÀM MỚI (Reset)
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ResetForm();
            LoadData(); // Load lại toàn bộ danh sách
        }

        // 7. CheckBox LỌC TỒN KHO (Logic LINQ tại chỗ)
        private void chkLocTonKho_CheckedChanged(object sender, EventArgs e)
        {
            // Kiểm tra nếu checkbox (do em mới tạo) được tick
            if (chkLocTonKho.Checked)
            {
                var list = spBUS.LayDanhSachSanPham();
                // Lọc các SP có số lượng > 0
                dgvSanPham.DataSource = list.Where(x => x.SoLuong > 0).ToList();
            }
            else
            {
                LoadData(); // Bỏ tick thì hiện lại tất cả
            }
        }

        // Sự kiện Click vào bảng
        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSanPham.Rows[e.RowIndex];
                txtMaSP.Text = row.Cells["MaSP"].Value.ToString();
                txtTenSP.Text = row.Cells["TenSP"].Value.ToString();
                txtDonGia.Text = row.Cells["DonGia"].Value.ToString();
                txtSoLuong.Text = row.Cells["SoLuong"].Value.ToString();

                // Kiểm tra null để tránh lỗi
                if (row.Cells["TrangThai"].Value != null)
                    chkTrangThai.Checked = (bool)row.Cells["TrangThai"].Value;
            }
        }

        // Hàm phụ xóa trắng TextBox
        private void ResetForm()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtDonGia.Text = "0";
            txtSoLuong.Text = "0";
            txtTimKiem.Clear();
            chkTrangThai.Checked = true;
        }

        private void TrangTriGiaoDien()
        {
            // 1. Màu nền Form
            this.BackColor = System.Drawing.Color.WhiteSmoke;

            // 2. Trang trí GroupBox
            groupBox1.BackColor = System.Drawing.Color.White;
            groupBox1.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Regular);
            groupBox1.ForeColor = System.Drawing.Color.DarkBlue;

            // 3. Trang trí DataGridView (Bảng dữ liệu)
            dgvSanPham.BackgroundColor = System.Drawing.Color.White;
            dgvSanPham.BorderStyle = BorderStyle.None;
            dgvSanPham.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(238, 239, 249);
            dgvSanPham.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSanPham.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.DarkTurquoise;
            dgvSanPham.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.WhiteSmoke;

            // Chỉnh màu tiêu đề cột
            dgvSanPham.EnableHeadersVisualStyles = false;
            dgvSanPham.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvSanPham.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(20, 25, 72); // Màu xanh đậm
            dgvSanPham.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvSanPham.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            dgvSanPham.ColumnHeadersHeight = 35; // Tăng chiều cao tiêu đề

            // 4. Trang trí các Nút bấm (Button)
            // Style nút THÊM (Màu xanh lá)
            StyleButton(btnThem, System.Drawing.Color.ForestGreen);

            // Style nút SỬA (Màu cam/vàng)
            StyleButton(btnSua, System.Drawing.Color.Goldenrod);

            // Style nút XÓA (Màu đỏ)
            StyleButton(btnXoa, System.Drawing.Color.Crimson);

            // Style nút LÀM MỚI (Màu xanh dương)
            StyleButton(btnLamMoi, System.Drawing.Color.SteelBlue);

            // Style nút TÌM KIẾM
            StyleButton(btnTimKiem, System.Drawing.Color.DimGray);
            // (Lưu ý: Trong code cũ của bạn nút tìm kiếm tên là button1, nếu bạn đổi tên rồi thì sửa lại nhé)
        }

        // Hàm phụ trợ để làm đẹp nút bấm
        private void StyleButton(Button btn, System.Drawing.Color color)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = System.Drawing.Color.White;
            btn.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            btn.Cursor = Cursors.Hand; // Đổi con trỏ chuột thành hình bàn tay
        }

    }
}