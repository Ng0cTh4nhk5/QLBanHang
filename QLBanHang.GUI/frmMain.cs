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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        // Sự kiện Form Load: Chạy ngay khi Form Main hiện lên
        private void frmMain_Load(object sender, EventArgs e)
        {
            // 1. Mở form Sản Phẩm (frmSanPham)
            frmSanPham fSP = new frmSanPham();
            fSP.MdiParent = this; // Đặt Form Main làm cha
            fSP.Text = "Danh mục Sản Phẩm";
            fSP.Show();

            // 2. Mở form Hóa Đơn (frmHoaDon)
            frmHoaDon fHD = new frmHoaDon();
            fHD.MdiParent = this; // Đặt Form Main làm cha
            fHD.Text = "Quản lý Hóa Đơn";
            fHD.Show();

            // 3. Tự động chia đôi màn hình dọc (Trái: SP, Phải: HD)
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        // Sự kiện khi bấm Menu "Sản Phẩm" (Nếu lỡ tắt muốn mở lại)
        private void sanPhamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem form đã mở chưa để tránh mở nhiều cái (Cơ bản)
            frmSanPham f = new frmSanPham();
            f.MdiParent = this;
            f.Show();
        }

        // Sự kiện khi bấm Menu "Hóa Đơn"
        private void hoaDonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmHoaDon f = new frmHoaDon();
            f.MdiParent = this;
            f.Show();
        }
    }
}
