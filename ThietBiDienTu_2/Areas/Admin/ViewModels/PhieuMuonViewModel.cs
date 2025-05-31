namespace ThietBiDienTu_2.Areas.Admin.ViewModels
{
    public class PhieuMuonViewModel
    {
        public int Mapm { get; set; }

        public DateTime Ngaymuon { get; set; }

        public DateTime Ngaylap { get; set; }

        public int? Manv { get; set; }

        public int Masv { get; set; }

        public int Trangthai { get; set; }

        public string Lydomuon { get; set; } = null!;

        public string? LydoTuChoi { get; set; }
        public string? LydoHuy {  get; set; }

        public string Tensv {  get; set; }
        public string TenKhoa {  get; set; }
        public string TenNganh {  get; set; }
        public List<ChitietPhieuMuonViewModel> ctpmView {  get; set; }
    }
}
