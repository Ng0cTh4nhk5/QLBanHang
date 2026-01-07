using System;
using System.Linq;
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
            TrangTriGiaoDien();
            // Mở rộng toàn màn hình
            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Hàm kiểm tra xem Form con đã mở hay chưa (Tránh mở trùng lặp)
        /// </summary>
        private Form KiemTraFormTonTai(Type formType)
        {
            return this.MdiChildren.FirstOrDefault(f => f.GetType() == formType);
        }

        /// <summary>
        /// Hàm chung để mở Form con
        /// </summary>
        private void MoFormCon(Type formType)
        {
            Form formCon = KiemTraFormTonTai(formType);
            if (formCon != null)
            {
                formCon.Activate(); // Nếu đã mở thì focus vào nó
            }
            else
            {
                // Tạo instance mới từ Type (Reflection)
                formCon = (Form)Activator.CreateInstance(formType);
                formCon.MdiParent = this;
                formCon.Show();
            }
        }

        // --- CÁC SỰ KIỆN MENU ---

        private void mnuSanPham_Click(object sender, EventArgs e)
        {
            MoFormCon(typeof(frmSanPham));
        }

        private void mnuHoaDon_Click(object sender, EventArgs e)
        {
            // Lưu ý: Đảm bảo bạn đã có class frmHoaDon trong project
            MoFormCon(typeof(frmHoaDon));
        }

        private void mnuThongKe_Click(object sender, EventArgs e)
        {
            MoFormCon(typeof(frmThongKe));
        }

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            // Xác nhận trước khi thoát
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // --- TRANG TRÍ GIAO DIỆN ---
        private void TrangTriGiaoDien()
        {
            // 1. MenuStrip
            menuStrip1.BackColor = System.Drawing.Color.FromArgb(20, 25, 72);
            menuStrip1.ForeColor = System.Drawing.Color.White;
            menuStrip1.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Regular);

            foreach (ToolStripMenuItem item in menuStrip1.Items)
            {
                item.ForeColor = System.Drawing.Color.White;
            }

            // 2. MDI Client (Vùng nền xám)
            foreach (Control ctl in this.Controls)
            {
                if (ctl is MdiClient)
                {
                    ctl.BackColor = System.Drawing.Color.WhiteSmoke;
                    break;
                }
            }

            this.Text = "HỆ THỐNG QUẢN LÝ BÁN HÀNG - PHIÊN BẢN 1.0";
        }
    }
}