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
    public partial class frmHoaDon : Form
    {
        // 1. KHAI BÁO CÁC BUS
        HoaDonBUS bus = new HoaDonBUS();
        SanPhamBUS spBUS = new SanPhamBUS();
        NhanVienBUS nvBUS = new NhanVienBUS();
        KhachHangBUS khachHangBUS = new KhachHangBUS();

        // Danh sách tạm để chứa chi tiết hóa đơn (Giỏ hàng)
        List<ChiTietHoaDonDTO> gioHang = new List<ChiTietHoaDonDTO>();

        public frmHoaDon()
        {
            InitializeComponent();
        }

        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCombobox();
                txtDonGia.ReadOnly = true;
                textBox1.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải Form: " + ex.Message);
            }
        }

        private void LoadCombobox()
        {
            // Load Khách hàng
            cboKhachHang.DataSource = khachHangBUS.LayDanhSachKhachHang();
            cboKhachHang.DisplayMember = "TenKH"; // Kiểm tra kỹ DTO property name
            cboKhachHang.ValueMember = "MaKH";

            // Load Nhân viên
            cboNhanVien.DataSource = nvBUS.LayDanhSachNhanVien();
            cboNhanVien.DisplayMember = "TenNV";
            cboNhanVien.ValueMember = "MaNV";

            // Load Sản phẩm
            cboSanPham.DataSource = spBUS.LayDanhSachSanPham();
            cboSanPham.DisplayMember = "TenSP";
            cboSanPham.ValueMember = "MaSP";
        }

        // Sự kiện khi chọn sản phẩm khác thì tự nhảy đơn giá tương ứng
        private void cboSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSanPham.SelectedItem == null) return;

            SanPhamDTO sp = cboSanPham.SelectedItem as SanPhamDTO;
            if (sp != null)
            {
                txtDonGia.Text = sp.DonGia.ToString("N0");
            }
        }

        // Nút THÊM VÀO GIỎ
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSoLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập số lượng!");
                return;
            }

            SanPhamDTO spChon = cboSanPham.SelectedItem as SanPhamDTO;

            if (!int.TryParse(txtSoLuong.Text, out int soLuongMua) || soLuongMua <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!");
                return;
            }

            if (spChon.SoLuong < soLuongMua)
            {
                MessageBox.Show($"Kho chỉ còn {spChon.SoLuong} sản phẩm!");
                return;
            }

            // Kiểm tra cộng dồn
            var itemTonTai = gioHang.FirstOrDefault(x => x.MaSP == spChon.MaSP);
            if (itemTonTai != null)
            {
                itemTonTai.SoLuong += soLuongMua;
            }
            else
            {
                ChiTietHoaDonDTO item = new ChiTietHoaDonDTO
                {
                    MaSP = spChon.MaSP,
                    TenSP = spChon.TenSP,
                    SoLuong = soLuongMua,
                    DonGia = spChon.DonGia
                };
                gioHang.Add(item);
            }

            HienThiGioHang();
        }

        private void HienThiGioHang()
        {
            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = gioHang;

            // Format hiển thị cột nếu cần
            if (dgvChiTiet.Columns["DonGia"] != null)
                dgvChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "N0";

            decimal tongTien = gioHang.Sum(ct => ct.SoLuong * ct.DonGia);
            lblTongTien.Text = "Tổng tiền: " + tongTien.ToString("N0") + " VNĐ";
        }

        private void btnInThu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra dữ liệu
            if (gioHang.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống, vui lòng chọn sản phẩm!");
                return;
            }
            if (cboKhachHang.SelectedIndex == -1 || cboNhanVien.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn Khách hàng và Nhân viên!");
                return;
            }

            try
            {
                // 2. Tạo Dataset ảo (Không cần gọi DB)
                dsHoaDon ds = new dsHoaDon();

                // --- Đổ dữ liệu Header (Thông tin chung) ---
                // Lấy Text hiển thị trên ComboBox (Tên KH) thay vì lấy Value (Mã KH)
                var rowHead = ds.dtHeader.NewdtHeaderRow();

                rowHead.MaHoaDon = 0; // Chưa lưu nên chưa có mã, để 0 hoặc để trống
                rowHead.NgayLap = DateTime.Now;
                rowHead.TenKhachHang = cboKhachHang.Text;
                rowHead.TenNhanVien = cboNhanVien.Text;

                // Các trường này nếu Form chưa có ô nhập thì để trống
                rowHead.DiaChi = "...................................";
                rowHead.SoDienThoai = "...................................";

                decimal tongTien = gioHang.Sum(x => x.SoLuong * x.DonGia);
                rowHead.TongTien = tongTien;
                rowHead.TongTienChu = "(Bằng chữ: ....................................................)";

                ds.dtHeader.AdddtHeaderRow(rowHead);

                // --- Đổ dữ liệu Detail (Danh sách hàng) ---
                int stt = 1;
                foreach (var item in gioHang)
                {
                    var rowDetail = ds.dtDetail.NewdtDetailRow();

                    rowDetail.MaHoaDon = 0;
                    rowDetail.STT = stt++;
                    rowDetail.TenSP = item.TenSP;
                    rowDetail.SoLuong = item.SoLuong;
                    rowDetail.DonGia = item.DonGia;
                    rowDetail.ThanhTien = item.SoLuong * item.DonGia;
                    rowDetail.DonViTinh = "Cái"; // Mặc định hoặc lấy từ DTO nếu có

                    ds.dtDetail.AdddtDetailRow(rowDetail);
                }

                // 3. Mở Form In với dữ liệu vừa tạo
                frmInHoaDon f = new frmInHoaDon(ds);
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo bản in thử: " + ex.Message);
            }
        }

        // Nút LƯU HÓA ĐƠN (Chỉ lưu khi đã chắc chắn)
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (gioHang.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống!");
                return;
            }

            if (cboKhachHang.SelectedIndex == -1 || cboNhanVien.SelectedIndex == -1)
            {
                MessageBox.Show("Chưa nhập đủ thông tin!");
                return;
            }

            // Hỏi xác nhận lần cuối
            DialogResult dr = MessageBox.Show("Bạn đã kiểm tra kỹ hóa đơn chưa? Nhấn Yes để Lưu.", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;

            // Tiến hành lưu
            HoaDonDTO hd = new HoaDonDTO();
            hd.NgayLap = DateTime.Now;
            hd.MaKH = Convert.ToInt32(cboKhachHang.SelectedValue);
            hd.MaNV = Convert.ToInt32(cboNhanVien.SelectedValue);
            hd.TongTien = gioHang.Sum(ct => ct.SoLuong * ct.DonGia);

            if (bus.LuuHoaDon(hd, gioHang))
            {
                MessageBox.Show("Lưu hóa đơn thành công! Kho đã được cập nhật.");

                // Reset form để bán đơn mới
                gioHang.Clear();
                dgvChiTiet.DataSource = null;
                lblTongTien.Text = "Tổng tiền: 0 VNĐ";
                txtSoLuong.Clear();
            }
            else
            {
                MessageBox.Show("Lưu thất bại! Vui lòng kiểm tra lại CSDL.");
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void txtDonGia_TextChanged(object sender, EventArgs e)
        {

        }
    }
}