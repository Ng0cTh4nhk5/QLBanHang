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
            TrangTriGiaoDien();
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

        private void TrangTriGiaoDien()
        {
            // 1. Trang trí Thanh Menu (MenuStrip)
            menuStrip1.BackColor = System.Drawing.Color.FromArgb(20, 25, 72); // Màu xanh đậm
            menuStrip1.ForeColor = System.Drawing.Color.White;
            menuStrip1.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Regular);

            // Duyệt qua từng menu con để chỉnh màu khi hiển thị
            foreach (ToolStripMenuItem item in menuStrip1.Items)
            {
                item.ForeColor = System.Drawing.Color.White;
            }

            // 2. Đổi màu nền vùng chứa MDI (Phần nền xám mặc định của Windows)
            // Mẹo: Vùng này thực chất là một control ẩn tên là "MdiClient"
            foreach (Control ctl in this.Controls)
            {
                if (ctl is MdiClient)
                {
                    // Đổi màu nền sang trắng khói cho sáng sủa, đồng bộ với Form con
                    ctl.BackColor = System.Drawing.Color.WhiteSmoke;
                    break; // Tìm thấy rồi thì dừng vòng lặp
                }
            }

            // 3. (Tuỳ chọn) Đặt tiêu đề Form cho chuyên nghiệp
            this.Text = "HỆ THỐNG QUẢN LÝ BÁN HÀNG - PHIÊN BẢN 1.0";
        }
    }
}