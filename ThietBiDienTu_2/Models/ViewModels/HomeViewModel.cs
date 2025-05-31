using ThietBiDienTu_2.Areas.Admin.ViewModels;

namespace ThietBiDienTu_2.Models.ViewModels
{
    public class HomeViewModel
    {
       

        public DateTime? NgayDat { get; set; }

       
        public List<Dongthietbi> DongThietBiList { get; set; }
        public List<Chitietphieumuon> ChiTietPhieuMuonList { get; set; } // Thêm thuộc tính ChiTietPhieuMuonList vào HomeViewModel
    }
}
