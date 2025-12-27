using System;
using System.Linq; // Để dùng hàm kiểm tra form mở rồi hay chưa
using System.Windows.Forms;

namespace QLBanHang.GUI
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Cho Form hiện giữa màn hình và to hết cỡ
            this.WindowState = FormWindowState.Maximized;
        }

        // --- HÀM KIỂM TRA FORM ĐÃ MỞ CHƯA (Tránh mở nhiều lần) ---
        private Form KiemTraFormTonTai(Type formType)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == formType)
                {
                    return f; // Trả về form nếu đã tìm thấy
                }
            }
            return null; // Trả về null nếu chưa mở
        }

        // --- 1. MỞ FORM SẢN PHẨM ---
        private void mnuSanPham_Click(object sender, EventArgs e)
        {
            Form f = KiemTraFormTonTai(typeof(frmSanPham));
            if (f != null)
            {
                f.Activate(); // Nếu mở rồi thì focus vào nó
            }
            else
            {
                frmSanPham fMoi = new frmSanPham();
                fMoi.MdiParent = this; // Đặt cha là frmMain
                fMoi.Show();
            }
        }

        // --- 2. MỞ FORM HÓA ĐƠN ---
        private void mnuHoaDon_Click(object sender, EventArgs e)
        {
            Form f = KiemTraFormTonTai(typeof(frmHoaDon));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                frmHoaDon fMoi = new frmHoaDon();
                fMoi.MdiParent = this;
                fMoi.Show();
            }
        }

        /*-- 3. MỞ FORM THỐNG KÊ(3 TRUY VẤN LINQ) ---*/
        private void mnuThongKe_Click(object sender, EventArgs e)
        {
            Form f = KiemTraFormTonTai(typeof(frmThongKe));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                frmThongKe fMoi = new frmThongKe();
                fMoi.MdiParent = this;
                fMoi.Show();
            }
        }

        // Nút Thoát
        private void mnuThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}