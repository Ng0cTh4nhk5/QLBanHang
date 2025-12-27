using QLBanHang.BUS;
using QLBanHang.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLBanHang.GUI
{
    public partial class frmSanPham : Form
    {
        SanPhamBUS spBUS = new SanPhamBUS();

        public frmSanPham()
        {
            InitializeComponent();
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            LoadData(); // Load dữ liệu khi mở form
        }

        // Hàm load dữ liệu lên Grid
        private void LoadData()
        {
            dgvSanPham.DataSource = spBUS.LayDanhSachSanPham();
        }

        // Sự kiện nút THÊM
        private void btnThem_Click(object sender, EventArgs e)
        {
            SanPhamDTO sp = new SanPhamDTO();
            sp.TenSP = txtTenSP.Text;
            sp.DonGia = decimal.Parse(txtDonGia.Text); // Cần try-catch nếu nhập sai
            sp.SoLuong = int.Parse(txtSoLuong.Text);
            sp.TrangThai = chkTrangThai.Checked;

            if (spBUS.ThemSanPham(sp))
            {
                MessageBox.Show("Thêm thành công!");
                LoadData();
            }
            else
            {
                MessageBox.Show("Thêm thất bại. Kiểm tra lại dữ liệu!");
            }
        }

        // Sự kiện Click vào dòng trong Grid để đổ dữ liệu ngược lại TextBox
        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy dòng hiện tại
                DataGridViewRow row = dgvSanPham.Rows[e.RowIndex];

                txtMaSP.Text = row.Cells["MaSP"].Value.ToString();
                txtTenSP.Text = row.Cells["TenSP"].Value.ToString();
                txtDonGia.Text = row.Cells["DonGia"].Value.ToString();
                txtSoLuong.Text = row.Cells["SoLuong"].Value.ToString();
                chkTrangThai.Checked = (bool)row.Cells["TrangThai"].Value;
            }
        }

        // Sự kiện nút XÓA
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSP.Text)) return;

            int maSP = int.Parse(txtMaSP.Text);
            if (spBUS.XoaSanPham(maSP))
            {
                MessageBox.Show("Xóa thành công!");
                LoadData();
            }
        }
    }
}