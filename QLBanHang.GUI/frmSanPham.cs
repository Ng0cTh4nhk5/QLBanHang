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
            LoadData();
        }

        // Hàm chung để tải dữ liệu
        private void LoadData()
        {
            dgvSanPham.DataSource = spBUS.LayDanhSachSanPham();
        }

        // 2. Nút THÊM
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                SanPhamDTO sp = new SanPhamDTO();
                sp.TenSP = txtTenSP.Text;
                sp.DonGia = decimal.Parse(txtDonGia.Text);
                sp.SoLuong = int.Parse(txtSoLuong.Text);
                sp.TrangThai = chkTrangThai.Checked;

                if (spBUS.ThemSanPham(sp))
                {
                    MessageBox.Show("Thêm thành công!");
                    LoadData();
                    ResetForm(); // Xóa trắng ô nhập
                }
                else
                {
                    MessageBox.Show("Thêm thất bại (Kiểm tra tên rỗng hoặc số âm)!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhập liệu: " + ex.Message);
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
    }
}