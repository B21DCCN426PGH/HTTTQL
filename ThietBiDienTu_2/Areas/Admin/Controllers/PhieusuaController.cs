using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Areas.Admin.ViewModels;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using ThietBiDienTu_2.Repository;
using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationM_S]
    public class PhieusuaController : Controller
    {
        IPhieuSuaAdmin psRepo; IThietBiAdmin tbRepo; IHttpContextAccessor contextAcc;

        public PhieusuaController(IPhieuSuaAdmin psRepo, IThietBiAdmin tbRepo, IHttpContextAccessor contextAcc)
        {
            this.psRepo = psRepo;
            this.tbRepo = tbRepo;
            this.contextAcc = contextAcc;
        }

        public IActionResult DanhsachPhieuSua(int? page,string? searchString,int? trangthai,DateTime? NgaylapTu,DateTime? NgaylapDen)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            List<Phieusua> psList = psRepo.GetAllPs().OrderByDescending(x=>x.Maps).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                psList = psList.Where(x=>x.Maps.ToString().Contains(searchString)).ToList();
            }

            if(trangthai > -1)
            {
                psList = psList.Where(x => x.Trangthai == trangthai).ToList();
                ViewBag.viewState = trangthai;
            }
            if (NgaylapTu.HasValue)
            {
                psList = psList.Where(x => x.Ngaylap >= NgaylapTu).ToList();
            }

            if (NgaylapDen.HasValue)
            {
                psList = psList.Where(x => x.Ngaylap <= NgaylapDen).ToList();
            }
            PagedList<Phieusua> psPaged = new PagedList<Phieusua>(psList, pageNumber, pageSize);

            if (IsAjaxRequest())
            {
                return PartialView("PartialViewDSPhieuSua", psPaged);
            }
            
            return View(psPaged);
        }

        private bool IsAjaxRequest()
        {
            var request = contextAcc.HttpContext.Request;
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        public IActionResult UpdatePS(int?pageView,int maps)
        {

            UpdatePSViewModel updatepsView = CreateUpdatePsView(pageView ?? 1, maps);
            return View(updatepsView);
        }

        public UpdatePSViewModel CreateUpdatePsView(int? pageView,int maps)
        {
            int page = pageView ?? 1;
            int pageSize = 5;

            Phieusua ps = psRepo.GetPsById(maps);
            List<TbFixAndCheck> tbFixCheck = ps.Chitietphieusuas.Select(x => new TbFixAndCheck
            {
                Matb = x.Matb,
                Seri = x.MatbNavigation.Seri,
                Hinhanh = x.MatbNavigation.MadongtbNavigation.Hinhanh,
                Tentb = x.MatbNavigation.MadongtbNavigation.Tendongtb,
                TenKho = x.MatbNavigation.MapNavigation.Tenphong,
                Makho = x.MatbNavigation.Map,
                CheckFix = x.Ngayhoanthanh.HasValue,
                NgayHoanThanh =x.Ngayhoanthanh,
                Chiphi = x.Chiphi ?? 0,
                Mota = x.Mota

            }).ToList();

            PagedList<TbFixAndCheck> pageTbFix = new PagedList<TbFixAndCheck>(tbFixCheck, pageView ?? 1, 5);

            UpdatePSViewModel updatepsView = new UpdatePSViewModel()
            {
                tbFixCheck = pageTbFix,
                Tongcong = tbFixCheck.Sum(x=>x.Chiphi),
                Maps = maps,
                NgayLap = ps.Ngaylap,
                Trangthai = ps.Trangthai
            };
            HttpContext.Session.SetJson("Fixcart", tbFixCheck);

            return updatepsView;
        }

        public IActionResult UpdatePsViewFinal(int maps)
        {
            List<TbFixAndCheck> tbFixCheck = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart");
            Phieusua ps = psRepo.GetPsById(maps);

            psRepo.UpdatePhieusua(tbFixCheck, ps);
            TempData["AlertMessage"] = "Phiếu sửa #" + maps+" vừa được cập nhật";
            return RedirectToAction("DanhsachPhieuSua");
        }
        public IActionResult updateInfoFixUpdate(int maps,int matb, decimal Chiphi, string Mota, int? pageChoosen)
        {
            Phieusua ps = psRepo.GetPsById(maps);
            List<TbFixAndCheck> tbFixCheck = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart");
            TbFixAndCheck tbFix = tbFixCheck.FirstOrDefault(x => x.Matb == matb);

            tbFix.Chiphi = Chiphi;
            tbFix.Mota = Mota;

            HttpContext.Session.SetJson("Fixcart", tbFixCheck);
            PagedList<TbFixAndCheck> pageTbFix = new PagedList<TbFixAndCheck>(tbFixCheck, pageChoosen ?? 1, 5);
            UpdatePSViewModel updatepsView = new UpdatePSViewModel()
            {
                tbFixCheck = pageTbFix,
                Tongcong = tbFixCheck.Sum(x => x.Chiphi),
                Maps = maps,
                NgayLap = ps.Ngaylap,
                Trangthai = ps.Trangthai
            };

            ViewBag.Pagechoosen = pageChoosen;
            return PartialView("PartialViewUpdateList", updatepsView);
        }

        public IActionResult ValidationUpdateInfoFixUpdate(int maps,int matb, decimal Chiphi, string Mota, int? pageChoosen)
        {
            List<TbFixAndCheck> tbFixCheck = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart");
            TbFixAndCheck tbFix = tbFixCheck.FirstOrDefault(x => x.Matb == matb);
            if (Chiphi == 0)
            {
                ViewBag.Chiphi = "Vui lòng điền chi phí";
            }

            ViewBag.ChiphiTemp = Chiphi;
            if (string.IsNullOrEmpty(Mota))
            {
                ViewBag.Mota = "Vui lòng điền mô tả sửa chữa";
            }
            else
            {
                tbFix.Mota = Mota;
            }

            Phieusua ps = psRepo.GetPsById(maps);

            HttpContext.Session.SetJson("Fixcart", tbFixCheck);
            PagedList<TbFixAndCheck> pageTbFix = new PagedList<TbFixAndCheck>(tbFixCheck, pageChoosen ?? 1, 5);
            UpdatePSViewModel updatepsView = new UpdatePSViewModel()
            {
                tbFixCheck = pageTbFix,
                Tongcong = tbFixCheck.Sum(x => x.Chiphi),
                Maps = maps,
                NgayLap = ps.Ngaylap,
                Trangthai = ps.Trangthai
            };

            ViewBag.Pagechoosen = pageChoosen;

            HttpContext.Session.SetJson("Fixcart", tbFixCheck);
            return PartialView("PartialViewUpdateList", updatepsView);
        }

        public IActionResult pageChoosenFilterUpdate(int maps,int pageChoosen)
        {
            Phieusua ps = psRepo.GetPsById(maps);
            List<TbFixAndCheck> tbFixCheck = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart");
            PagedList<TbFixAndCheck> pageTbFix = new PagedList<TbFixAndCheck>(tbFixCheck, pageChoosen, 5);
            UpdatePSViewModel updatepsView = new UpdatePSViewModel()
            {
                tbFixCheck = pageTbFix,
                Tongcong = tbFixCheck.Sum(x => x.Chiphi),
                Maps = maps,
                NgayLap = ps.Ngaylap,
                Trangthai = ps.Trangthai
            };

            ViewBag.Pagechoosen = pageChoosen;
            return PartialView("PartialViewUpdateList", updatepsView);
        }

        public IActionResult ChangeState(int maps,int matb,bool checkValue)
        {
            Phieusua ps = psRepo.GetPsById(maps);
            List<TbFixAndCheck> tbFixCheck = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart");
            TbFixAndCheck tbFix = tbFixCheck.FirstOrDefault(x => x.Matb == matb);
            tbFix.CheckFix = checkValue;

            HttpContext.Session.SetJson("Fixcart", tbFixCheck);
            int checkStateAll = tbFixCheck.Any(x => x.CheckFix == false) ? 0 : 1;

            return PartialView("PartialViewTrangthaisua", checkStateAll);
        }

        public IActionResult CreatePs()
        {
            //AddFix(1161);
            //ConfirmTemp();
            HttpContext.Session.Remove("FixcartTemp");
            HttpContext.Session.Remove("Fixcart");
            CreatePSViewModel createPs = GetCreatePsAndCountAndData(false,1,1,null,null);
           
            return View(createPs);
        }

        public IActionResult CreatePsFinal()
        {
            List<TbFixAndCheck> tbChoosenList = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart") ?? new List<TbFixAndCheck>();
            if(tbChoosenList.Count == 0)
            {
                ViewBag.Themtb = "Vui lòng chọn thiết bị để sửa";
                CreatePSViewModel createPs = GetCreatePsAndCountAndData(false, 1, 1, null, null);
                return View("CreatePs", createPs);
            }
            if(tbChoosenList.Any(x=>x.Chiphi == 0))
            {
                ViewBag.AddChiphi = "Có thiết bị chưa điền chi phí hoặc mô tả sửa chữa";
                CreatePSViewModel createPs = GetCreatePsAndCountAndData(false, 1, 1, null, null);
                return View("CreatePs", createPs);
            }

            

            int maps = psRepo.CreatePs(tbChoosenList, tbChoosenList.Sum(x => x.Chiphi));
            TempData["AlertMessage"] = "Phiếu sửa vừa tạo có mã: " + maps;
            return RedirectToAction("DanhsachPhieuSua");
        }


        public IActionResult FilterAddFix(int? pageAdd,int? pagechoosen, string? searchString,string? makho,bool? check)
        {
            CreatePSViewModel createPs = GetCreatePsAndCountAndData(check, pageAdd ?? 1, pagechoosen ?? 1,searchString, makho);
           
            ViewBag.page = pageAdd;
            ViewBag.searchString = searchString;
            ViewBag.makho = makho;
            ViewBag.check = check;
            return PartialView("_PartialViewAddFix", createPs.tbList);
        }

        public IActionResult pageChoosenFilter(int pageChoosen)
        {
            CreatePSViewModel createPs = GetCreatePsAndCountAndData(false, 1, pageChoosen, null, null);

            return PartialView("_PartialViewShowChoosen",createPs);
        }

        public CreatePSViewModel GetCreatePsAndCountAndData(bool? keepCheck,int pageTbFixView,int? pageChoosen,string? searchString,string? makho)
        {
            List<TbFixAndCheck> tbFixList = tbRepo.GetTbFixAndCheckList();
            List<TbFixAndCheck> tbChoosenList = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart") ?? new List<TbFixAndCheck>();
            List<TbFixAndCheck> tbChoosenListTemp = HttpContext.Session.GetJson<List<TbFixAndCheck>>("FixcartTemp") ?? new List<TbFixAndCheck>();
            for(int i=0;i< tbFixList.Count();++i)
            {
                if (tbChoosenListTemp.FirstOrDefault(x=>x.Matb == tbFixList[i].Matb)!=null)
                {
                    tbFixList[i].CheckFix = true;
                }
            }

            var count = tbFixList.Count();
            for (int i= count - 1; i >= 0; --i)
            {
                if (tbChoosenList.FirstOrDefault(x => x.Matb == tbFixList[i].Matb) != null)
                {
                    tbFixList.Remove(tbFixList[i]);
                }
            }

            List<SelectListItem> selectKho = tbFixList.Select(t => new SelectListItem
            {
                Value = t.Makho,
                Text = t.Makho +" - "+t.TenKho
            }).GroupBy(x => x.Value).Select(x => x.First()).ToList();
            ViewBag.selectKhoList = selectKho;
            ViewBag.Pagechoosen = pageChoosen ?? 1;
            if (keepCheck == true)
            {
                count = tbFixList.Count();
                for (int i= count-1; i>= 0; --i)
                {
                    if (tbChoosenListTemp.FirstOrDefault(x => x.Matb == tbFixList[i].Matb) != null)
                    {
                        tbFixList[i].CheckFix = true;
                    }
                    else
                    {
                        tbFixList.Remove(tbFixList[i]);
                    }
                }
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                tbFixList = tbFixList.Where(x => x.Tentb.ToLower().Contains(searchString.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(makho))
            {
                tbFixList = tbFixList.Where(x=>x.Makho == makho).ToList();
            }
            

            PagedList<TbFixAndCheck> pageTbFixList = new PagedList<TbFixAndCheck>(tbFixList, pageTbFixView, 5);
            PagedList<TbFixAndCheck> pageTbChoosenList = new PagedList<TbFixAndCheck>(tbChoosenList, pageChoosen ?? 1, 5);

            CreatePSViewModel createPs = new CreatePSViewModel
            {
                tbList = pageTbFixList,
                tbChoosen = pageTbChoosenList,
                Tongchiphi = tbChoosenList.Sum(x=>x.Chiphi)
            };
            return createPs;
        }

        public void AddFix(int matb)
        {
            List<TbFixAndCheck> tbFixList = HttpContext.Session.GetJson<List<TbFixAndCheck>>("FixcartTemp") ?? new List<TbFixAndCheck>();
            Thietbi tb = tbRepo.GetTBById(matb);
            tbFixList.Add(new TbFixAndCheck
            {
                Hinhanh = tb.MadongtbNavigation.Hinhanh,
                TenKho = tb.MapNavigation.Tenphong,
                Makho = tb.Map,
                Matb = matb,
                Seri = tb.Seri,
                Tentb = tb.MadongtbNavigation.Tendongtb,
                CheckFix = true,
                Chiphi = 0,
                Mota = ""
            });
            HttpContext.Session.SetJson("FixcartTemp", tbFixList);
        }

        public void RemoveFix(int matb)
        {
            List<TbFixAndCheck> tbFixList = HttpContext.Session.GetJson<List<TbFixAndCheck>>("FixcartTemp") ?? new List<TbFixAndCheck>();
            TbFixAndCheck tb = tbFixList.FirstOrDefault(x => x.Matb == matb);

            tbFixList.Remove(tb);
            if (tbFixList.Count() == 0)
            {
                HttpContext.Session.Remove("FixcartTemp");
            }
            else
            {
                HttpContext.Session.SetJson("FixcartTemp", tbFixList);
            }
        }
    
        public IActionResult closeAddTb(int? pageChoosen)
        {
            HttpContext.Session.Remove("FixcartTemp");

            CreatePSViewModel createPs = GetCreatePsAndCountAndData(false, 1, pageChoosen ?? 1, null, null);

            return PartialView("_PartialViewAddFix", createPs.tbList);
        }
    
        //public void ConfirmTemp()
        //{
        //    List<TbFixAndCheck> tbChoosenList = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart") ?? new List<TbFixAndCheck>();
        //    List<TbFixAndCheck> tbChoosenListTemp = HttpContext.Session.GetJson<List<TbFixAndCheck>>("FixcartTemp");

        //    foreach (TbFixAndCheck tbFix in tbChoosenListTemp)
        //    {
        //        tbChoosenList.Add(tbFix);
        //    }
        //    HttpContext.Session.SetJson("Fixcart", tbChoosenList);
        //    HttpContext.Session.Remove("FixcartTemp");
        //}

        public IActionResult ConfirmTb(int? pageChoosen)
        {
            List<TbFixAndCheck> tbChoosenList = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart") ?? new List<TbFixAndCheck>();
            List<TbFixAndCheck> tbChoosenListTemp = HttpContext.Session.GetJson<List<TbFixAndCheck>>("FixcartTemp");

            foreach(TbFixAndCheck tbFix in tbChoosenListTemp)
            {
                tbChoosenList.Add(tbFix);
            }
            HttpContext.Session.SetJson("Fixcart", tbChoosenList);
            HttpContext.Session.Remove("FixcartTemp");

            CreatePSViewModel createPs = GetCreatePsAndCountAndData(false, 1, pageChoosen ?? 1,null, null);


            return PartialView("_PartialViewShowChoosen", createPs);
        }

        public IActionResult DeleteFromFixCart(int matb,int? pageChoosen)
        {
            List<TbFixAndCheck> tbChoosenList = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart") ?? new List<TbFixAndCheck>();
            TbFixAndCheck tbDelete = tbChoosenList.FirstOrDefault(x => x.Matb == matb);
            tbChoosenList.Remove(tbDelete);

            HttpContext.Session.SetJson("Fixcart", tbChoosenList);
            CreatePSViewModel createPs = GetCreatePsAndCountAndData(false, 1, pageChoosen ?? 1 , null, null);
            return PartialView("_PartialViewShowChoosen", createPs);
        }

        public IActionResult updateInfoFix(int matb, decimal Chiphi, string Mota,int? pageChoosen)
        {

            List<TbFixAndCheck> tbChoosenList = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart");
            TbFixAndCheck tbFix = tbChoosenList.FirstOrDefault(x => x.Matb == matb);

            tbFix.Chiphi = Chiphi;
            tbFix.Mota = Mota;

            HttpContext.Session.SetJson("Fixcart", tbChoosenList);
            CreatePSViewModel createPs = GetCreatePsAndCountAndData(false, 1, pageChoosen ?? 1, null, null);
            return PartialView("_PartialViewShowChoosen", createPs);
        }
    
        public IActionResult ValidationUpdateInfoFix(int matb,decimal  Chiphi, string Mota,int? pageChoosen)
        {
            List<TbFixAndCheck> tbChoosenList = HttpContext.Session.GetJson<List<TbFixAndCheck>>("Fixcart");
            TbFixAndCheck tbFix = tbChoosenList.FirstOrDefault(x => x.Matb == matb);
            if (Chiphi == 0)
            {
                ViewBag.Chiphi = "Vui lòng điền chi phí";
            }
            else
            {
                ViewBag.ChiphiTemp = Chiphi;
            }
            if (string.IsNullOrEmpty(Mota))
            {
                ViewBag.Mota = "Vui lòng điền mô tả sửa chữa";
            }
            else
            {
                tbFix.Mota = Mota;
            }
            HttpContext.Session.SetJson("Fixcart", tbChoosenList);
            CreatePSViewModel createPs = GetCreatePsAndCountAndData(false, 1, pageChoosen ?? 1, null, null);
            return PartialView("_PartialViewShowChoosen", createPs);
        }


        public IActionResult DeletePs(int id)
        {
            if(psRepo.Deletepheusua(id))
            {
                TempData["AlertMessage"] = "Phiếu sửa đã được xóa";
            }
            else
            {
                TempData["AlertMessageFail"] = "Phiếu sửa đã hoàn thành không thể xóa";
            }
            return RedirectToAction("DanhsachPhieuSua");
        }
    }
}
