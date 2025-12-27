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
        // Khai báo BUS để dùng cho trường hợp in từ CSDL
        private HoaDonBUS hoaDonBUS = new HoaDonBUS();

        // Biến lưu mã hóa đơn (Dùng cho trường hợp IN LẠI hóa đơn cũ)
        private int _maHoaDon = 0;

        // Biến lưu dữ liệu nguồn (Dùng cho trường hợp IN THỬ / PREVIEW)
        private dsHoaDon _dsNguon = null;

        // ---------------------------------------------------------
        // CONSTRUCTOR 1: Dùng khi IN THỬ (Nhận Dataset trực tiếp)
        // ---------------------------------------------------------
        public frmInHoaDon(dsHoaDon ds)
        {
            InitializeComponent();
            _dsNguon = ds; // Hứng lấy dữ liệu mang sang
        }

        // ---------------------------------------------------------
        // CONSTRUCTOR 2: Dùng khi IN THẬT (Nhận Mã HĐ để tìm trong DB)
        // ---------------------------------------------------------
        public frmInHoaDon(int maHD)
        {
            InitializeComponent();
            _maHoaDon = maHD;
        }

        private void frmInHoaDon_Load(object sender, EventArgs e)
        {

            try
            {
                // ƯU TIÊN 1: CHẾ ĐỘ PREVIEW (IN THỬ)
                // Nếu biến _dsNguon có dữ liệu -> Hiển thị ngay và DỪNG LẠI (Return)
                if (_dsNguon != null)
                {
                    HienThiBaoCao(_dsNguon);
                    return; // <--- Quan trọng: Không chạy code bên dưới nữa
                }

                // ƯU TIÊN 2: CHẾ ĐỘ IN TỪ DATABASE
                // Nếu không có dữ liệu nguồn -> Gọi BUS đi tìm trong CSDL theo Mã HĐ
                if (_maHoaDon > 0)
                {
                    dsHoaDon dsDB = hoaDonBUS.LayDuLieuInHoaDon(_maHoaDon);

                    if (dsDB.dtHeader.Rows.Count > 0)
                    {
                        HienThiBaoCao(dsDB);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin hóa đơn số: " + _maHoaDon);
                        this.Close(); // Đóng form luôn nếu không thấy
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị báo cáo: " + ex.Message);
            }
        }

        // Hàm chung để đẩy dữ liệu lên Report Viewer
        private void HienThiBaoCao(dsHoaDon ds)
        {
            // 1. Reset lại report
            this.reportViewer1.Reset();

            // 2. GỌI ĐÚNG TÊN FILE REPORT
      
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "QLBanHang.GUI.rptHoaDon.rdlc";

            // 3. Gán dữ liệu 
            this.reportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource rdsHeader = new ReportDataSource("dtsHeader", (DataTable)ds.dtHeader);
            ReportDataSource rdsDetail = new ReportDataSource("dtsDetail", (DataTable)ds.dtDetail);

            this.reportViewer1.LocalReport.DataSources.Add(rdsHeader);
            this.reportViewer1.LocalReport.DataSources.Add(rdsDetail);

            this.reportViewer1.RefreshReport();
        }
    }
}