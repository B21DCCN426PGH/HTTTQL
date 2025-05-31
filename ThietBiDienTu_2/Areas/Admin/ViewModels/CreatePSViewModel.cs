using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.ViewModels
{
    public class CreatePSViewModel
    {
        public PagedList<TbFixAndCheck> tbList {  get; set; }
        public PagedList<TbFixAndCheck> tbChoosen {  get; set; }  
        public decimal Tongchiphi {  get; set; }
    }
}
