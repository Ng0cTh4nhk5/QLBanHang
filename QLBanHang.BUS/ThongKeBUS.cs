using QLBanHang.DAL;
using System;

namespace QLBanHang.BUS
{
    public class ThongKeBUS
    {
        ThongKeDAL dal = new ThongKeDAL();

        public object LayDsHoaDonTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            return dal.LayDsHoaDonTheoNgay(tuNgay, denNgay);
        }

        public object LayTop3SanPham()
        {
            return dal.LayTop3SanPhamBanChay();
        }

        public object LayDoanhThuTheoThang()
        {
            return dal.LayDoanhThuTheoThang();
        }
    }
}