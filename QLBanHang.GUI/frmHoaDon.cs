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
        HoaDonBUS bus = new HoaDonBUS();
        SanPhamBUS spBUS = new SanPhamBUS(); // Để lấy danh sách SP
        NhanVienBUS nvBUS = new NhanVienBUS();

        // Danh sách tạm để chứa chi tiết hóa đơn (Giỏ hàng)
        List<ChiTietHoaDonDTO> gioHang = new List<ChiTietHoaDonDTO>();

        public frmHoaDon()
        {
            InitializeComponent();
        }

        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            LoadCombobox();
            txtDonGia.ReadOnly = true; // Không cho sửa đơn giá

            textBox1.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // textBox1.ReadOnly = true;
        }

        private void LoadCombobox()
        {
            // Đổ dữ liệu vào ComboBox Khách hàng
            cboKhachHang.DataSource = bus.LayDanhSachKhachHang();
            cboKhachHang.DisplayMember = "TenKH";
            cboKhachHang.ValueMember = "MaKH";


            // 2. Gọi hàm lấy dữ liệu 
            cboNhanVien.DataSource = nvBUS.LayDanhSachNhanVien();

            // 3. Quan trọng: Kiểm tra bên DTO nhân viên, thuộc tính tên là gì?
            // Ví dụ: public string HoTen { get; set; } thì DisplayMember phải là "HoTen"
            cboNhanVien.DisplayMember = "TenNV"; // Hoặc "HoTen" tùy DTO của bạn
            cboNhanVien.ValueMember = "MaNV";

            // Đổ dữ liệu vào ComboBox Sản phẩm
            cboSanPham.DataSource = spBUS.LayDanhSachSanPham();
            cboSanPham.DisplayMember = "TenSP";
            cboSanPham.ValueMember = "MaSP";
        }

        // Sự kiện khi chọn sản phẩm khác thì tự nhảy đơn giá tương ứng
        private void cboSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            SanPhamDTO sp = cboSanPham.SelectedItem as SanPhamDTO;
            if (sp != null)
            {
                txtDonGia.Text = sp.DonGia.ToString();
            }
        }

        // Nút THÊM VÀO GIỎ
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSoLuong.Text)) return;

            SanPhamDTO spChon = cboSanPham.SelectedItem as SanPhamDTO;
            int soLuongMua = int.Parse(txtSoLuong.Text);

            // Kiểm tra tồn kho (Logic phụ thêm cho xịn)
            if (soLuongMua > spChon.SoLuong)
            {
                MessageBox.Show("Kho không đủ hàng!");
                return;
            }

            // Tạo chi tiết mới
            ChiTietHoaDonDTO item = new ChiTietHoaDonDTO
            {
                MaSP = spChon.MaSP,
                TenSP = spChon.TenSP,
                SoLuong = soLuongMua,
                DonGia = spChon.DonGia
            };

            // Thêm vào list tạm
            gioHang.Add(item);

            // Cập nhật lại Grid
            HienThiGioHang();
        }

        private void HienThiGioHang()
        {
            // Reset nguồn dữ liệu
            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = gioHang;

            // TÍNH TỔNG TIỀN BẰNG LINQ (Yêu cầu bắt buộc của đề bài) 
            decimal tongTien = gioHang.Sum(ct => ct.SoLuong * ct.DonGia);

            lblTongTien.Text = "Tổng tiền: " + tongTien.ToString("N0") + " VNĐ";
        }

        // Nút LƯU HÓA ĐƠN (Gửi dữ liệu xuống BUS)
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (gioHang.Count == 0) return;

            // Tạo đối tượng Hóa đơn
            HoaDonDTO hd = new HoaDonDTO
            {
                NgayLap = DateTime.Now,
                MaKH = (int)cboKhachHang.SelectedValue,
                MaNV = 1 // (int)cboNhanVien.SelectedValue - Tạm thời fix cứng nếu chưa làm cboNV
            };

            if (bus.LuuHoaDon(hd, gioHang))
            {
                MessageBox.Show("Lập hóa đơn thành công!");
                gioHang.Clear(); // Xóa giỏ hàng để làm mới
                HienThiGioHang();
            }
            else
            {
                MessageBox.Show("Có lỗi khi lưu hóa đơn!");
            }
        }
    }
}