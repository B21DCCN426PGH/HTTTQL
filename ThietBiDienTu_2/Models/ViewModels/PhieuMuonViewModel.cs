namespace ThietBiDienTu_2.Models.ViewModels
{
    public class PhieuMuonViewModel
    {
        public int Mapm { get; set; }
        public int? Manv { get; set; }
        public DateTime? NgayDat { get; set; }
        public DateTime? Ngaylap { get; set; }
        public int Masv { get; set; }

        public int Trangthai { get; set; }

        public string Lydomuon { get; set; } = null!;

        public string? LydoTuChoi { get; set; }

        public string? LydoHuy { get; set; }
        public string Tensv { get; set; }
        public string TenKhoa { get; set; }
        public string TenNganh { get; set; }
        public List<Chitietphieumuon> ChiTietPhieuMuonList { get; set; } // Thêm thuộc tính ChiTietPhieuMuonList vào HomeViewModel
        public List<ChitietPhieuMuonViewModel> ctpmView { get; set; }
    }
}
