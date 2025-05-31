////using Microsoft.AspNetCore.Mvc;
////using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
////using ThietBiDienTu_2.Areas.Admin.ViewModels;
////using ThietBiDienTu_2.Models;
////using X.PagedList;

////namespace ThietBiDienTu_2.Areas.Admin.ViewComponents
////{
////    public class ChooseDeviceViewComponent : ViewComponent
////    {
////        IDongThietBiAdmin dongtbRepo;
////        public ChooseDeviceViewComponent(IDongThietBiAdmin dongtbRepo)
////        {
////            this.dongtbRepo = dongtbRepo;
////        }

////        public IViewComponentResult Invoke(int? page, string? searchString)
////        {
////            int pageSize = 5;
////            int pageNumber = page == null || page < 0 ? 1 : page.Value;
           
////            List<Dongthietbi> dongtbList = dongtbRepo.DongTbAndAmountTbInDay(DateTime.Parse("2024-04-27"));
////            PagedList<Dongthietbi> pagedongtb = new PagedList<Dongthietbi>(dongtbList,pageNumber,pageSize);
////            return View(pagedongtb);
////        }
////    }
////}
