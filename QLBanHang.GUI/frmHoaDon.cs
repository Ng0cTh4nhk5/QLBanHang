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

        // --- SỬA LỖI 1: Thêm dòng này để khởi tạo KhachHangBUS ---
        KhachHangBUS khachHangBUS = new KhachHangBUS();
        // ---------------------------------------------------------

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
            // --- SỬA LỖI 2: Gọi qua đối tượng 'khachHangBUS' thay vì gọi tên Class ---
            cboKhachHang.DataSource = khachHangBUS.LayDanhSachKhachHang();

            // LƯU Ý QUAN TRỌNG: Kiểm tra kỹ file KhachHangDTO.cs
            // Nếu property là 'TenKhachHang' thì sửa bên dưới thành "TenKhachHang"
            // Nếu property là 'TenKH' thì giữ nguyên "TenKH"
            cboKhachHang.DisplayMember = "TenKH";
            cboKhachHang.ValueMember = "MaKH";
            // -------------------------------------------------------------------------

            // Load Nhân viên
            cboNhanVien.DataSource = nvBUS.LayDanhSachNhanVien();
            cboNhanVien.DisplayMember = "TenNV"; // Kiểm tra lại DTO NhanVien xem là TenNV hay HoTen
            cboNhanVien.ValueMember = "MaNV";

            // Load Sản phẩm
            cboSanPham.DataSource = spBUS.LayDanhSachSanPham();
            cboSanPham.DisplayMember = "TenSP";
            cboSanPham.ValueMember = "MaSP";
        }

        // Sự kiện khi chọn sản phẩm khác thì tự nhảy đơn giá tương ứng
        private void cboSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra null để tránh lỗi khi form mới load
            if (cboSanPham.SelectedItem == null) return;

            SanPhamDTO sp = cboSanPham.SelectedItem as SanPhamDTO;
            if (sp != null)
            {
                txtDonGia.Text = sp.DonGia.ToString("N0"); // Format số cho đẹp
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

            // Dùng TryParse để tránh lỗi crash nếu người dùng nhập chữ
            if (!int.TryParse(txtSoLuong.Text, out int soLuongMua) || soLuongMua <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!");
                return;
            }

            // Kiểm tra tồn kho (Giả sử DTO có trường SoLuong)
            if (spChon.SoLuong < soLuongMua)
            {
                MessageBox.Show($"Kho chỉ còn {spChon.SoLuong} sản phẩm!");
                return;
            }

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa để cộng dồn (Logic nâng cao)
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
            // Reset DataSource để Grid cập nhật lại giao diện
            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = gioHang;

            // Format cột đơn giá trên Grid cho đẹp (nếu cần)
            // dgvChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "N0";

            decimal tongTien = gioHang.Sum(ct => ct.SoLuong * ct.DonGia);
            lblTongTien.Text = "Tổng tiền: " + tongTien.ToString("N0") + " VNĐ";
        }

        // Nút LƯU HÓA ĐƠN
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (gioHang.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống!");
                return;
            }

            // Kiểm tra giá trị null an toàn hơn
            if (cboKhachHang.SelectedIndex == -1)
            {
                MessageBox.Show("Chưa chọn khách hàng!");
                return;
            }
            if (cboNhanVien.SelectedIndex == -1)
            {
                MessageBox.Show("Chưa chọn nhân viên!");
                return;
            }

            // Tính tổng tiền
            decimal tongTien = gioHang.Sum(ct => ct.SoLuong * ct.DonGia);

            HoaDonDTO hd = new HoaDonDTO();
            hd.NgayLap = DateTime.Now;

            // Ép kiểu an toàn từ ValueMember
            hd.MaKH = Convert.ToInt32(cboKhachHang.SelectedValue);
            hd.MaNV = Convert.ToInt32(cboNhanVien.SelectedValue);
            hd.TongTien = tongTien;

            if (bus.LuuHoaDon(hd, gioHang))
            {
                MessageBox.Show("Lưu hóa đơn thành công!");

                // Reset form
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
    }
}