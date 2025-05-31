using ThietBiDienTu_2.Models;
using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.ViewModels
{
    public class CreatePmViewModel
    {
        public string Tensv {  get; set; }
        public string TenKhoa {  get; set; }
        public string TenNganh { get; set; }
        public DateTime NgayLap = DateTime.Now.Date;

        public PagedList<DongTbAndAmount> pagedongtb;
    }
}
