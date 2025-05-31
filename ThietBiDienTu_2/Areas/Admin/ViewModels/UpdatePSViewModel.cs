using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.ViewModels
{
    public class UpdatePSViewModel
    {
        public PagedList<TbFixAndCheck> tbFixCheck {  get; set; }
        public decimal Tongcong {  get; set; }
        public DateTime NgayLap {  get; set; }
        public int Maps {  get; set; }

        public int Trangthai {  get; set; }
    }
}
