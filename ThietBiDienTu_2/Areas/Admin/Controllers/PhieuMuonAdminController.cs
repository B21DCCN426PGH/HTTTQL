using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Areas.Admin.Repositories;
using ThietBiDienTu_2.Areas.Admin.ViewModels;
using ThietBiDienTu_2.Migrations;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using ThietBiDienTu_2.Repository;
using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [AuthenticationM_S]
    public class PhieuMuonAdminController : Controller
    {
        IPhieuMuonAdmin pMAdmin; IHttpContextAccessor contextAcc; IKhoa kRepo;INhanVien nvRepo;
        ISinhvienAdmin svRepo; IDongThietBiAdmin dongtbRepo; IThietBiAdmin tbRepo; IPhongAdmin roomRepo;
        public PhieuMuonAdminController(IPhieuMuonAdmin _pMAdmin,IHttpContextAccessor contextAcc,IKhoa kRepo,
            INhanVien nvRepo,ISinhvienAdmin svRepo,IDongThietBiAdmin dongtbRepo,IThietBiAdmin tpRepo,
            IPhongAdmin roomRepo)
        {
            pMAdmin = _pMAdmin;
            this.contextAcc = contextAcc;
            this.kRepo = kRepo;
            this.nvRepo= nvRepo;
            this.svRepo = svRepo;
            this.dongtbRepo = dongtbRepo;
            this.tbRepo = tpRepo;
            this.roomRepo = roomRepo; 
        }

        public IActionResult DanhsachPhieuMuon(int? page, string? searchString, string? Trangthai,int? indexPartial,DateTime? From,DateTime? To)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            PagedList<Phieumuon> pm;
            List<Phieumuon> pmList = pMAdmin.GetAllPhieuMuon().OrderBy(x => x.Trangthai)
                                                .ThenByDescending(x=>x.Mapm).ToList();
            CreateData(searchString, Trangthai,From,To);

            DateTime today = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(searchString))
            {
                ViewBag.searchString = searchString;
                pmList = pmList.Where(x => x.Masv.ToString().Contains(searchString)
                                    || x.Mapm.ToString().Contains(searchString)).ToList();
            }
            if (From.HasValue)
            {
                pmList = pmList.Where(x => x.Ngaymuon >= From).ToList();
            }
            if (To.HasValue)
            {

                pmList = pmList.Where(x => x.Ngaymuon.Date <= To.Value.Date).ToList();
            }
            if (!string.IsNullOrEmpty(Trangthai) && Trangthai != "-1")
            {
                pmList = pmList.Where(x => x.Trangthai.ToString() == Trangthai).ToList();
            }


            if (IsAjaxRequest())
            {
                if (indexPartial == 1)
                {
                    pmList = pmList.Where(x => x.Ngaymuon == today).ToList();
                    pm = new PagedList<Phieumuon>(pmList, pageNumber, pageSize);
                    return PartialView("_PartialTableToday",pm);
                }
                if (indexPartial == 2) //Not checked - not give back
                {
                    pm = new PagedList<Phieumuon>(pmList, pageNumber, pageSize);
                    return PartialView("_PartialTableAnother",pm);
                }
            }
            if(indexPartial == 1)
            {
                ViewBag.indexPartial = 1;
            }
            return View(pmList);
        }

        public IActionResult ChitietPhieuMuon(int mapm)
        {

            PhieuMuonViewModel pm = pMAdmin.GetPhieumuonViewById(mapm);

            return View(pm);
        }

        [HttpPost]
        public IActionResult ChitietPhieuMuon(PhieuMuonViewModel pm, int trangthai)
        {
         
            pm.Manv = HttpContext.Session.GetInt32("UserName") ?? 0;
            if(trangthai == 4 && pm.LydoTuChoi == null)
            {
                PhieuMuonViewModel pmReturn = pMAdmin.GetPhieumuonViewById(pm.Mapm);
                ViewBag.error = true;
                ModelState.AddModelError("LydoTuChoi", "Vui lòng chọn lý do từ chối");
                return View(pmReturn);
            }
			pMAdmin.DuyetPm(trangthai, pm);
            TempData["Duyet"] = "Duyệt thành công";
            return RedirectToAction("DanhsachPhieuMuon");
        }

        public IActionResult CreatePm(int? masv,DateTime Ngaymuon,string? Lydomuon,int ajaxCall,bool checkInBorrow=false)
        {
            List<DongTbAndAmount> dongtbList = countDongTbAndAmount(Ngaymuon);
            
            if (checkInBorrow == false)
            {
                HttpContext.Session.Remove("BorrowCart");
                HttpContext.Session.Remove("NgaymuonPm");
            }
           
            PagedList<DongTbAndAmount> pagedongtb = new PagedList<DongTbAndAmount>(dongtbList, 1, 5);
            CreatePmViewModel crPmview = new CreatePmViewModel();
            crPmview.pagedongtb = pagedongtb;

            if (IsAjaxRequest())
            {
                
                if (ajaxCall==1)
                {
                    Sinhvien sv = svRepo.GetSvById(masv ?? 0);
                    if (sv != null)
                    {
                        crPmview.TenNganh = sv.ManganhNavigation.Tennganh;
                        crPmview.TenKhoa = sv.MakhoaNavigation.Tenkhoa;
                        crPmview.Tensv = sv.Tensv;
                    }
                    else
                    {
                        ViewBag.NotFindsv = true;
                    }
                    return PartialView("_PartialViewCreatePm", crPmview);
                }

                checkNgayMuonSame(Ngaymuon);
                return PartialView("_PartialViewChooseDevices", pagedongtb);
            }
            return View(crPmview);
        }

        [HttpPost]
        public IActionResult CreatePm(int masv, DateTime Ngaymuon, string? Lydomuon)
        {

            if(checkValidCreatePm(masv, Ngaymuon, Lydomuon) == false)
            {
                List<DongTbAndAmount> dongtbList = countDongTbAndAmount(Ngaymuon);
                PagedList<DongTbAndAmount> pagedongtb = new PagedList<DongTbAndAmount>(dongtbList, 1, 5);
                CreatePmViewModel crPmview = new CreatePmViewModel();
                crPmview.pagedongtb = pagedongtb;
                ViewBag.LydomuonView = Lydomuon;
                ViewBag.MasvView = masv;

                Sinhvien sv = svRepo.GetSvById(masv);
                if (sv != null)
                {
                    crPmview.TenNganh = sv.ManganhNavigation.Tennganh;
                    crPmview.TenKhoa = sv.MakhoaNavigation.Tenkhoa;
                    crPmview.Tensv = sv.Tensv;
                }
                else
                {
                    ViewBag.NotFindsv = true;
                }
                return View(crPmview);
            }

            List<DongTbAndAmount> dongtbAmountList = HttpContext.Session.GetJson<List<DongTbAndAmount>>("BorrowCart");
            int manv = HttpContext.Session.GetInt32("UserName") ?? 0;
            int mapm = pMAdmin.CreatePm(masv, Ngaymuon, Lydomuon, dongtbAmountList,manv);

            TempData["Duyet"] = "Phiếu mượn mới tạo có mã " + mapm;

            return RedirectToAction("DanhsachPhieuMuon");
        }

        public bool checkValidCreatePm(int masv, DateTime Ngaymuon, string Lydomuon)
        {
            bool check = true;
            Sinhvien sv = svRepo.GetSvById(masv);
            if(sv == null)
            {
                ViewBag.MasvValid = true;
                check = false;
            }
            if(Ngaymuon.Year < 2010)
            {
                ViewBag.NgayMuonValid = true;
                check = false;
            }
            if (string.IsNullOrEmpty(Lydomuon))
            {
                ViewBag.LydomuonValid = true;
                check = false;
            }
            if (HttpContext.Session.GetJson<List<DongTbAndAmount>>("BorrowCart") == null)
            {
                ViewBag.BorrowCartValid = true;
                check = false;
            }
            return check;
        }

        public IActionResult AddBorrowDevices(int madongtb, int? page, string? searchString, DateTime Ngaymuon)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            ViewBag.pageAdd = page;

            checkNgayMuonSame(Ngaymuon);

            List<DongTbAndAmount> BorrowCart = HttpContext.Session.GetJson<List<DongTbAndAmount>>("BorrowCart") ?? new List<DongTbAndAmount>();
            
            if (madongtb !=0)
            {
                DongTbAndAmount dongtbAmount = BorrowCart.FirstOrDefault(x => x.madongtb == madongtb);

                if (dongtbAmount != null)
                {
                    dongtbAmount.amount += 1;
                }
                else
                {
                    Dongthietbi dongtb = dongtbRepo.GetDtbById(madongtb);
                    dongtbAmount = new DongTbAndAmount()
                    {
                        madongtb = madongtb,
                        tendongtb = dongtb.Tendongtb,
                        hinhanh = dongtb.Hinhanh,
                        amount = 1
                    };
                    BorrowCart.Add(dongtbAmount);

                }
                contextAcc.HttpContext.Session.SetJson("BorrowCart", BorrowCart);
            }
            List<DongTbAndAmount> dongtbAmountList = countDongTbAndAmount(Ngaymuon);

            if (!string.IsNullOrEmpty(searchString))
            {
                dongtbAmountList = dongtbAmountList.Where(x => x.tendongtb.ToLower().Contains(searchString.ToLower())).ToList();
                ViewBag.searchStringAdd = searchString;
            }

            PagedList<DongTbAndAmount> pagedongtb = new PagedList<DongTbAndAmount>(dongtbAmountList, pageNumber, pageSize);

            return PartialView("_PartialViewChooseDevices",pagedongtb);
        }

        public IActionResult AddQuantity(int madongtb)
        {
            List<DongTbAndAmount> BorrowCart = HttpContext.Session.GetJson<List<DongTbAndAmount>>("BorrowCart") ?? new List<DongTbAndAmount>();
            string NgaymuonString = HttpContext.Session.GetString("NgaymuonPm");
            DateTime Ngaymuon = DateTime.ParseExact(NgaymuonString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            List<DongTbAndAmount> dongtbAmountList = countDongTbAndAmount(Ngaymuon);

            DongTbAndAmount dongtbAmount = dongtbAmountList.FirstOrDefault(x => x.madongtb == madongtb);
            if (dongtbAmount == null)
            {
                ViewBag.LimitQuantityAdd = true;
            }
            else
            {
                BorrowCart.FirstOrDefault(x => x.madongtb == madongtb).amount += 1;
                HttpContext.Session.SetJson("BorrowCart", BorrowCart);

                dongtbAmountList = countDongTbAndAmount(Ngaymuon);
            }


            PagedList<DongTbAndAmount> pagedongtb = new PagedList<DongTbAndAmount>(dongtbAmountList, 1, 5);

            return PartialView("_PartialViewChooseDevices", pagedongtb);

        }

        public IActionResult MinusQuantity(int madongtb)
        {
            List<DongTbAndAmount> BorrowCart = HttpContext.Session.GetJson<List<DongTbAndAmount>>("BorrowCart") ?? new List<DongTbAndAmount>();
            string NgaymuonString = HttpContext.Session.GetString("NgaymuonPm");
            DateTime Ngaymuon = DateTime.ParseExact(NgaymuonString, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            DongTbAndAmount dongtbAmount = BorrowCart.FirstOrDefault(x => x.madongtb == madongtb);
            dongtbAmount.amount -= 1;
            if (dongtbAmount.amount == 0)
            {
                BorrowCart.Remove(dongtbAmount);
            }
            
            if (BorrowCart.Count == 0)
            {
                HttpContext.Session.Remove("BorrowCart");
            }
            else
            {
                HttpContext.Session.SetJson("BorrowCart", BorrowCart);
            }
            List<DongTbAndAmount> dongtbAmountList = countDongTbAndAmount(Ngaymuon);

            PagedList<DongTbAndAmount> pagedongtb = new PagedList<DongTbAndAmount>(dongtbAmountList, 1, 5);

            return PartialView("_PartialViewChooseDevices", pagedongtb);

        }

        public IActionResult DeleteQuantity(int madongtb)
        {
            List<DongTbAndAmount> BorrowCart = HttpContext.Session.GetJson<List<DongTbAndAmount>>("BorrowCart") ?? new List<DongTbAndAmount>();
            string NgaymuonString = HttpContext.Session.GetString("NgaymuonPm");
            DateTime Ngaymuon = DateTime.ParseExact(NgaymuonString, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            BorrowCart.Remove(BorrowCart.FirstOrDefault(x => x.madongtb == madongtb));
            if (BorrowCart.Count == 0)
            {
                HttpContext.Session.Remove("BorrowCart");
            }
            else
            {
                HttpContext.Session.SetJson("BorrowCart", BorrowCart);
            }

            List<DongTbAndAmount> dongtbAmountList = countDongTbAndAmount(Ngaymuon);

            PagedList<DongTbAndAmount> pagedongtb = new PagedList<DongTbAndAmount>(dongtbAmountList, 1, 5);

            return PartialView("_PartialViewChooseDevices", pagedongtb);

        }

        public IActionResult ChangeDevices(int? page,string?searchString,string? makho,int matb,int mapm)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            Phieumuon pm = pMAdmin.GetPmById(mapm);

            ViewBag.MatbOld = matb; 
            ViewBag.Mapm = mapm;
            Thietbi tb = tbRepo.GetTBById(matb);
            ViewBag.Madongtb = tb.Madongtb;

            List<Thietbi> tbList = tbRepo.GetTbListInThatDay(matb, pm.Ngaymuon);

            List <Phong> khoList = roomRepo.phongKhoListOfTbList(tbList);
            List<SelectListItem> selectKhoList = khoList.Select(x => new SelectListItem
            {
                Value = x.Map,
                Text = x.Map + " - " + x.Tenphong
            }).ToList();

            ViewBag.selectKhoList = selectKhoList;
            if (!string.IsNullOrEmpty(searchString))
            {
                tbList = tbList.Where(x => x.Seri.ToLower().Contains(searchString.ToLower())).ToList();
                ViewBag.searchStringView = searchString;
            }
            if (!string.IsNullOrEmpty(makho))
            {
                tbList = tbList.Where(x => x.Map == makho).ToList();
                ViewBag.selectKhoView = makho;
            }

            PagedList<Thietbi> tbPageList = new PagedList<Thietbi>(tbList, pageNumber, pageSize);


            return PartialView("_PartialViewChangeDevices", tbPageList);
        }

        public IActionResult ChangeDevicesDecided(int matbOld,int matbNew,int mapm)
        {
            ViewBag.Alert = true;
            pMAdmin.ReplaceDevices(matbOld,matbNew,mapm);
            PhieuMuonViewModel pm = pMAdmin.GetPhieumuonViewById(mapm);
            return PartialView("_PartialViewCtpmList", pm);
        }

        public void checkNgayMuonSame(DateTime Ngaymuon)
        {
            string NgaymuonSession = HttpContext.Session.GetString("NgaymuonPm") ?? "01-11-2004";
            DateTime NgayMuonOld = DateTime.ParseExact(NgaymuonSession, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            if (Ngaymuon != NgayMuonOld)
            {
                if(Ngaymuon.Year < 2010)
                {
                    HttpContext.Session.SetString("NgaymuonPm", "01-11-2004");
                }
                else
                {
                    HttpContext.Session.SetString("NgaymuonPm", Ngaymuon.ToString("dd-MM-yyyy"));
                }
                HttpContext.Session.Remove("BorrowCart");
            }
        }

        public List<DongTbAndAmount> countDongTbAndAmount(DateTime Ngaymuon)
        {
            List<DongTbAndAmount> dongtbList;
            List<DongTbAndAmount> BorrowCart = HttpContext.Session.GetJson<List<DongTbAndAmount>>("BorrowCart") ?? new List<DongTbAndAmount>();
            if (Ngaymuon.Year > 2010)
            {
                dongtbList = dongtbRepo.DongTbAndAmountTbInDay(Ngaymuon);
                foreach (DongTbAndAmount bCart in BorrowCart)
                {
                    DongTbAndAmount dongtbAmount = dongtbList.FirstOrDefault(x => x.madongtb == bCart.madongtb);
                    dongtbAmount.amount -= bCart.amount;
                    if(dongtbAmount.amount == 0)
                    {
                        dongtbList.Remove(dongtbAmount);
                    }
                    
                }
            }
            else
            {
                dongtbList = new List<DongTbAndAmount>();
            }
            return dongtbList;
        }
        
        public void CreateData(string? searchString,string? Trangthai,DateTime? From,DateTime? To)
        {
            List<int> trangthaiToday = pMAdmin.AllStatePhieuMuonToday();
            List<SelectListItem> selectState = trangthaiToday.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x == 0 ? "Chưa duyệt" : x == 1 ? "Chưa trả" : x == 2 ? "Đã duyệt" : x == 3 ? "Đã trả" : x == 4 ? "Từ chối" : x == 5 ? "Hủy phiếu" : ""
            }).ToList();
            ViewBag.StateSelect = selectState;
            ViewBag.searchString1 = searchString;
            ViewBag.selectState = Trangthai ?? "-1";
            ViewBag.borrowFrom = From.HasValue ? From.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.borrowTo = To.HasValue ? To.Value.ToString("yyyy-MM-dd") : "";
        }

        private bool IsAjaxRequest()
        {
            var request = contextAcc.HttpContext.Request;
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
