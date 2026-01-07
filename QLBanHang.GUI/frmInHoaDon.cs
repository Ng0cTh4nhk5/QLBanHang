using Microsoft.Reporting.WinForms;
using QLBanHang.BUS;
using QLBanHang.DTO;
using System;
using System.Data;
using System.Windows.Forms;

namespace QLBanHang.GUI
{
    public partial class frmInHoaDon : Form
    {
        private readonly HoaDonBUS _hoaDonBUS;

        // Dùng để lưu mã hóa đơn khi muốn in lại từ DB
        private readonly int _maHoaDon = 0;

        // Dùng để lưu dataset khi muốn in thử (Preview)
        private readonly dsHoaDon _dsNguon = null;

        /// <summary>
        /// Constructor 1: Dùng cho chế độ PREVIEW (Không cần truy vấn DB)
        /// </summary>
        public frmInHoaDon(dsHoaDon ds)
        {
            InitializeComponent();
            _dsNguon = ds;
        }

        /// <summary>
        /// Constructor 2: Dùng cho chế độ IN LẠI (Truy vấn theo Mã HĐ)
        /// </summary>
        public frmInHoaDon(int maHD)
        {
            InitializeComponent();
            _hoaDonBUS = new HoaDonBUS();
            _maHoaDon = maHD;
        }

        private void frmInHoaDon_Load(object sender, EventArgs e)
        {
            try
            {
                // TRƯỜNG HỢP 1: IN THỬ (Có dữ liệu nguồn truyền qua)
                if (_dsNguon != null)
                {
                    HienThiBaoCao(_dsNguon);
                    return;
                }

                // TRƯỜNG HỢP 2: IN TỪ CSDL
                if (_maHoaDon > 0)
                {
                    dsHoaDon dsDB = _hoaDonBUS.LayDuLieuInHoaDon(_maHoaDon);

                    if (dsDB.dtHeader.Rows.Count > 0)
                    {
                        HienThiBaoCao(dsDB);
                    }
                    else
                    {
                        MessageBox.Show($"Không tìm thấy hóa đơn #{_maHoaDon}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị báo cáo: {ex.Message}");
            }
        }

        private void HienThiBaoCao(dsHoaDon ds)
        {
            // 1. Reset ReportViewer
            this.reportViewer1.Reset();

            // 2. Trỏ đến file Report (đảm bảo file .rdlc nằm đúng thư mục)
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "QLBanHang.GUI.rptHoaDon.rdlc";

            // 3. Xóa nguồn dữ liệu cũ
            this.reportViewer1.LocalReport.DataSources.Clear();

            // 4. Map DataTable vào DataSource của Report
            // Lưu ý: "dtsHeader" và "dtsDetail" phải trùng tên Dataset Name bạn đặt trong file rdlc thiết kế
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dtsHeader", (DataTable)ds.dtHeader));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dtsDetail", (DataTable)ds.dtDetail));

            // 5. Render
            this.reportViewer1.RefreshReport();
        }
    }
}