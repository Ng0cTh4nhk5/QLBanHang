using QLBanHang.BUS;
using QLBanHang.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace QLBanHang.GUI
{
    public partial class frmHoaDon : Form
    {
        // 1. KHAI BÁO BUS (Chuẩn hóa tên biến private readonly)
        private readonly HoaDonBUS _hoaDonBUS;
        private readonly SanPhamBUS _sanPhamBUS;
        private readonly NhanVienBUS _nhanVienBUS;
        private readonly KhachHangBUS _khachHangBUS;

        // Giỏ hàng lưu tạm trong RAM
        private List<ChiTietHoaDonDTO> _gioHang;

        public frmHoaDon()
        {
            InitializeComponent();

            // Khởi tạo các đối tượng trong Constructor
            _hoaDonBUS = new HoaDonBUS();
            _sanPhamBUS = new SanPhamBUS();
            _nhanVienBUS = new NhanVienBUS();
            _khachHangBUS = new KhachHangBUS();
            _gioHang = new List<ChiTietHoaDonDTO>();
        }

        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            try
            {
                TrangTriGiaoDien();
                LoadDataCombobox();

                // Set mặc định
                txtDonGia.ReadOnly = true;
                txtSoLuong.Text = "1";
                textBox1.Text = DateTime.Now.ToString("dd/MM/yyyy"); // Chỉ hiển thị, không lưu
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải Form: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataCombobox()
        {
            // 1. Load Khách hàng
            cboKhachHang.DataSource = _khachHangBUS.LayDanhSachKhachHang();
            cboKhachHang.DisplayMember = "TenKH";
            cboKhachHang.ValueMember = "MaKH";
            cboKhachHang.SelectedIndex = -1; // Không chọn ai mặc định

            // 2. Load Nhân viên
            cboNhanVien.DataSource = _nhanVienBUS.LayDanhSachNhanVien();
            cboNhanVien.DisplayMember = "TenNV";
            cboNhanVien.ValueMember = "MaNV";
            cboNhanVien.SelectedIndex = -1;

            // 3. Load Sản phẩm
            cboSanPham.DataSource = _sanPhamBUS.LayDanhSachSanPham();
            cboSanPham.DisplayMember = "TenSP";
            cboSanPham.ValueMember = "MaSP";
            cboSanPham.SelectedIndex = -1;
        }

        // Sự kiện chọn sản phẩm -> Tự nhảy giá
        private void cboSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSanPham.SelectedItem is SanPhamDTO sp)
            {
                txtDonGia.Text = sp.DonGia.ToString("N0");
                txtSoLuong.Focus(); // Nhảy con trỏ xuống ô số lượng cho tiện
            }
        }

        // --- NÚT THÊM VÀO GIỎ ---
        private void btnThem_Click(object sender, EventArgs e)
        {
            // 1. Validation
            if (cboSanPham.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Cảnh báo");
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuongMua) || soLuongMua <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Cảnh báo");
                return;
            }

            // 2. Kiểm tra tồn kho logic
            SanPhamDTO spChon = cboSanPham.SelectedItem as SanPhamDTO;

            // Nếu sản phẩm đã có trong giỏ -> Cộng dồn số lượng hiện tại + số lượng định mua
            var itemTrongGio = _gioHang.FirstOrDefault(x => x.MaSP == spChon.MaSP);
            int soLuongHienTaiTrongGio = itemTrongGio != null ? itemTrongGio.SoLuong : 0;

            if (spChon.SoLuong < (soLuongMua + soLuongHienTaiTrongGio))
            {
                MessageBox.Show($"Kho chỉ còn {spChon.SoLuong} sản phẩm (Giỏ hàng đã có {soLuongHienTaiTrongGio})!", "Hết hàng");
                return;
            }

            // 3. Thêm vào List
            if (itemTrongGio != null)
            {
                itemTrongGio.SoLuong += soLuongMua;
            }
            else
            {
                _gioHang.Add(new ChiTietHoaDonDTO
                {
                    MaSP = spChon.MaSP,
                    TenSP = spChon.TenSP,
                    SoLuong = soLuongMua,
                    DonGia = spChon.DonGia
                });
            }

            // 4. Cập nhật giao diện
            HienThiGioHang();
        }

        private void HienThiGioHang()
        {
            // Reset DataSource để lưới cập nhật lại dữ liệu mới nhất trong List
            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = _gioHang;

            // Format cột tiền tệ
            if (dgvChiTiet.Columns["DonGia"] != null) dgvChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            if (dgvChiTiet.Columns["ThanhTien"] != null) dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";

            // Tính tổng tiền hiển thị
            decimal tongTien = _gioHang.Sum(x => x.ThanhTien);
            lblTongTien.Text = $"TỔNG TIỀN: {tongTien:N0} VNĐ";
        }

        // --- NÚT LƯU HÓA ĐƠN ---
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (_gioHang.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống!", "Cảnh báo");
                return;
            }

            if (cboKhachHang.SelectedIndex == -1 || cboNhanVien.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn Khách hàng và Nhân viên!", "Cảnh báo");
                return;
            }

            if (MessageBox.Show("Xác nhận thanh toán và lưu hóa đơn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    HoaDonDTO hd = new HoaDonDTO
                    {
                        NgayLap = DateTime.Now,
                        MaKH = (int)cboKhachHang.SelectedValue,
                        MaNV = (int)cboNhanVien.SelectedValue,
                        TongTien = _gioHang.Sum(x => x.ThanhTien)
                    };

                    if (_hoaDonBUS.LuuHoaDon(hd, _gioHang))
                    {
                        MessageBox.Show("Lưu hóa đơn thành công! Kho đã được cập nhật.", "Thành công");
                        ResetForm();
                    }
                    else
                    {
                        MessageBox.Show("Lưu thất bại! Có lỗi xảy ra trong quá trình xử lý.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi hệ thống: {ex.Message}");
                }
            }
        }

        private void ResetForm()
        {
            _gioHang.Clear();
            dgvChiTiet.DataSource = null;
            lblTongTien.Text = "TỔNG TIỀN: 0 VNĐ";
            txtSoLuong.Text = "1";
            cboSanPham.SelectedIndex = -1;
            cboKhachHang.SelectedIndex = -1;
        }

        // --- NÚT IN THỬ / PREVIEW ---
        private void btnInThu_Click(object sender, EventArgs e)
        {
            if (_gioHang.Count == 0 || cboKhachHang.SelectedIndex == -1 || cboNhanVien.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin và chọn sản phẩm để in thử!", "Thông báo");
                return;
            }

            try
            {
                // Gọi hàm tạo Dataset ảo
                dsHoaDon ds = TaoDuLieuInThu();

                // Mở Form In với dữ liệu vừa tạo (Constructor 1)
                frmInHoaDon f = new frmInHoaDon(ds);
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo bản in: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo Dataset giả lập từ giao diện mà không cần lưu xuống DB
        /// </summary>
        private dsHoaDon TaoDuLieuInThu()
        {
            dsHoaDon ds = new dsHoaDon();

            // 1. Tạo Header
            var rowHead = ds.dtHeader.NewdtHeaderRow();
            rowHead.MaHoaDon = 0; // In thử chưa có mã
            rowHead.NgayLap = DateTime.Now;
            rowHead.TenKhachHang = cboKhachHang.Text;
            rowHead.TenNhanVien = cboNhanVien.Text;
            rowHead.DiaChi = "Khách vãng lai"; // Demo
            rowHead.SoDienThoai = "N/A";
            rowHead.TongTien = _gioHang.Sum(x => x.ThanhTien);
            rowHead.TongTienChu = "(Chưa hỗ trợ đọc số thành chữ)";
            ds.dtHeader.AdddtHeaderRow(rowHead);

            // 2. Tạo Detail
            int stt = 1;
            foreach (var item in _gioHang)
            {
                var rowDetail = ds.dtDetail.NewdtDetailRow();
                rowDetail.MaHoaDon = 0;
                rowDetail.STT = stt++;
                rowDetail.TenSP = item.TenSP;
                rowDetail.SoLuong = item.SoLuong;
                rowDetail.DonGia = item.DonGia;
                rowDetail.ThanhTien = item.ThanhTien;
                ds.dtDetail.AdddtDetailRow(rowDetail);
            }

            return ds;
        }

        // Trang trí giao diện 
        private void TrangTriGiaoDien()
        {
            lblTongTien.ForeColor = System.Drawing.Color.Red;
            lblTongTien.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
        }
    }
}