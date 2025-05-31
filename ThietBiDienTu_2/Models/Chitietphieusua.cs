namespace ThietBiDienTu_2.Models
{
    public class Chitietphieusua
    {
        public int Maps { get; set; }
        public int Matb { get; set; }
        public DateTime? Ngayhoanthanh { get; set; }
        public string? Mota { get; set; }
        public Decimal? Chiphi { get; set;}
        public virtual Thietbi MatbNavigation { get; set; }
        public virtual Phieusua MapsNavigation { get; set; }

    }
}
