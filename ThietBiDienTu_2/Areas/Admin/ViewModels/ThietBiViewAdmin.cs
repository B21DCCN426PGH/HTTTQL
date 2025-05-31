using System.ComponentModel.DataAnnotations;

namespace ThietBiDienTu_2.Areas.Admin.ViewModels
{
    public class ThietBiViewAdmin
    {
        [Required(ErrorMessage = "Vui lòng chọn phòng")]
        public int Matb {  get; set; }
        public string Seri {  get; set; }
        public string HinhAnh {  get; set; }
        public string DongThietBi {get; set; }
        [Required(ErrorMessage = "Vui lòng chọn phòng")]
        public int MaDongTb { get; set; } 
        public string CoSo {  get; set; }

        [Required(ErrorMessage = "Vui lòng chọn cơ sở")]
        public int MaCoSo {  get; set; }
        public string Phong {  get; set; }
        public string MaP { get; set; }
        public string LoaiPhong {  get; set; }
        public string TrangThai {  get; set; }
    }
}
