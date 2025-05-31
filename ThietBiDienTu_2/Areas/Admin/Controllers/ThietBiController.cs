using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Areas.Admin.ViewModels;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [AuthenticationM_S]
    public class ThietBiController : Controller
    {
        ICoSoAdmin coso; IPhongAdmin pRepo; IDongThietBiAdmin Dongtb;
        IThietBiAdmin thietbi; IPhieuMuonAdmin pmRepo; IPhieuSuaAdmin psRepo;
        IHttpContextAccessor contextAccess;
        public ThietBiController(ICoSoAdmin CS, IPhongAdmin p, IDongThietBiAdmin _dongtb,
            IThietBiAdmin tb, IPhieuMuonAdmin pmRepo,IPhieuSuaAdmin psRepo,IHttpContextAccessor contextAccessor)
        {
            coso = CS;
            pRepo = p;
            Dongtb = _dongtb;
            thietbi = tb;
            this.pmRepo = pmRepo;
            this.psRepo = psRepo;
            this.contextAccess = contextAccessor;
        }

        public IActionResult ThietBiList(int? page, string? searchStringThietBi, string? Coso, string? Phong, string? LoaiPhong, string? Trangthai)
        {
            Debug.WriteLine("search string thietbi list: ", searchStringThietBi);
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            PagedList<ThietBiViewAdmin> ThietBiView;
            List<Coso> csList = coso.GetCoSoList();
            List<Thietbi> tb = thietbi.GetTBList();
            List<ThietBiViewAdmin> dataList = tb.Select(x => new ThietBiViewAdmin
            {
                Matb = x.Matb,
                Seri = x.Seri,
                HinhAnh = x.MadongtbNavigation.Hinhanh,
                DongThietBi = x.MadongtbNavigation.Tendongtb,
                MaDongTb = x.Madongtb,
                MaP = x.Map,
                Phong = x.MapNavigation.Tenphong,
                LoaiPhong = x.MapNavigation.Loaiphong,
                CoSo = csList.FirstOrDefault(y => y.Macs == x.MapNavigation.Macs).Tencs,
                MaCoSo = x.MapNavigation.Macs,
                TrangThai = x.Trangthai
            }).ToList();

      

            if (!string.IsNullOrEmpty(searchStringThietBi))
            {
                dataList = dataList.Where(x => x.DongThietBi != null && 
                (x.DongThietBi.ToLower().Contains(searchStringThietBi.TrimEnd().ToLower())  || x.Seri.ToLower().Contains(searchStringThietBi.TrimEnd().ToLower())  )).ToList();
                ViewBag.searchStringThietBi= searchStringThietBi;
            }
            if (!string.IsNullOrEmpty(Coso) && Coso != "all")
            {
                dataList = dataList.Where(x => x.MaCoSo.ToString() == Coso).ToList();
                
            }
            if (!string.IsNullOrEmpty(Phong) && Phong != "all")
            {
                dataList = dataList.Where(x => x.MaP == Phong).ToList();
                
            }
            
            if (!string.IsNullOrEmpty(LoaiPhong) && LoaiPhong != "all")
            {
                dataList = dataList.Where(x => x.LoaiPhong == LoaiPhong).ToList();
              
            }
            if (!string.IsNullOrEmpty(Trangthai) && Trangthai != "all")
            {
                dataList = dataList.Where(x => x.TrangThai == Trangthai).ToList();
                ViewBag.State = Trangthai;


            }
            dataList = dataList.OrderByDescending(x => x.Matb).ToList();
            CreateSelectData();
            ThietBiView = new PagedList<ThietBiViewAdmin>(dataList, pageNumber, pageSize);

            if (IsAjaxRequest())
            {
                return PartialView("PartialViewTBList", ThietBiView);
            }
            //ThietbiList = ThietbiList.OrderBy(x => x.Tensv);
            return View(ThietBiView);
        }
        private bool IsAjaxRequest()
        {
            var request = contextAccess.HttpContext.Request;
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
        public IActionResult CreateThietBi()
        {
            CreateSelectData();
            return View();
        }

        [HttpPost]
        public IActionResult CreateThietBi(ThietBiViewAdmin ThietBiView)
        {
            
            CreateSelectData();
            if (CheckValid(ThietBiView) ==1)
            {
                return View(ThietBiView);
            }
            TempData["Action"] = "Tạo thành công";
            Thietbi tb = new Thietbi
            {
                Matb = ThietBiView.Matb,
                Seri = ThietBiView.Seri,
                Map = ThietBiView.MaP,
                Madongtb = ThietBiView.MaDongTb,
                Trangthai = ThietBiView.TrangThai
            };
            thietbi.AddTB(tb);

            return RedirectToAction("ThietBiList");
        }

        public IActionResult UpdateThietBi(int Id)
        {
            CreateSelectData();
            Thietbi tb = thietbi.GetTBById(Id);
            List<Coso> cs = coso.GetCoSoList();
            ThietBiViewAdmin tDetail = new ThietBiViewAdmin
            {
                Matb = tb.Matb,
                Seri = tb.Seri,
                MaCoSo = cs.FirstOrDefault(x => x.Macs == tb.MapNavigation.Macs).Macs,
                MaP = tb.Map,
                MaDongTb = tb.Madongtb,
                LoaiPhong = tb.MapNavigation.Loaiphong,
               HinhAnh = tb.MadongtbNavigation.Hinhanh,
               DongThietBi = tb.MadongtbNavigation.Tendongtb,
               TrangThai = tb.Trangthai
            };

            ViewBag.oldSeri = tb.Seri;
            return View(tDetail);
        }

        [HttpPost]
        public IActionResult UpdateThietBi(ThietBiViewAdmin ThietBiView,string oldSeri)
        {
            
            CreateSelectData();
            ViewBag.oldSeri = oldSeri;
            if (CheckValid(ThietBiView, oldSeri) == 1)
            {
                return View(ThietBiView);
            }
            TempData["Action"] = "Cập nhật thành công";
            
            thietbi.UpdateTB(ThietBiView);
            return RedirectToAction("ThietBiList") ;
        }

        public IActionResult DeleteThietBi(int id)
        {
            if(psRepo.TbHasPhieuSua(id)==true || pmRepo.TbHasPhieuMuon(id)==true)
            {
                TempData["Fail"] = "Thiết bị không thể xóa";
            }
            else
            {
                TempData["Action"] = "Thiết bị đã được xóa";
                thietbi.DeleteTB(id);
            }
            return RedirectToAction("ThietBiList");
        }

        public int CheckValid(ThietBiViewAdmin ThietBiView,string oldSeri= "")
        {
            int check = 0;
            var ExistingCtsp = thietbi.CheckSeriExist(ThietBiView.Seri,ThietBiView.MaDongTb,oldSeri);

            if (ExistingCtsp != null)
            {
                ModelState.AddModelError("Seri", "Seri của thiết bị này đã tồn tại");
                check = 1;
            }
            if (ThietBiView.MaP == "all")
            {
                ModelState.AddModelError("MaP", "Vui lòng chọn phòng");
                check = 1;
            }
            if (ThietBiView.MaCoSo == 0)
            {
                ModelState.AddModelError("MaCoSo", "Vui lòng chọn cơ sở");
                check = 1;
            }
            if (ThietBiView.MaDongTb == 0)
            {
                ModelState.AddModelError("MaTb", "Vui lòng chọn thiết bị");
                check = 1;
            }
            if (!ModelState.IsValid)
            {
                check = 1;
            }
            if (ThietBiView.MaDongTb !=0 && oldSeri == "")
            {
                if (Dongtb.CountSlFullOrNot(ThietBiView.MaDongTb))
                {
                    ViewBag.FullAmount = "Số lượng của thiết bị đã đạt giới hạn";
                    check = 1;
                }              
            }
            
            return check;
        }

        public IActionResult getPhongList(string macs)
        {
            List<Phong> roomList = new List<Phong>();
            if (macs == "all")
            {
                roomList = pRepo.GetPhongList();
            }
            else
            {
                roomList = pRepo.GetPhongListByCoso(macs);
            }
            List<SelectListItem> phongListItem = roomList.Select(p => new SelectListItem
            {
                Value = p.Map.ToString(),
                Text = p.Map.ToString() + "-" + p.Tenphong ,
            }).ToList();

            return Json(phongListItem);
        }

        public IActionResult getLoaiPhongList(string map)
        {
            List<SelectListItem> loaiphongListItem = new List<SelectListItem>();
            if (map == "all")
            {
                List<Phong> roomList = pRepo.GetPhongList();
                List<string> stringLoaiPhong = roomList.Select(p => p.Loaiphong).Distinct().ToList();
                loaiphongListItem = stringLoaiPhong.Select(p => new SelectListItem
                {
                    Value = p,
                    Text = p
                }).ToList();
                
            }
            else
            {
                Phong phong = pRepo.FindPhong(map);
                loaiphongListItem.Add(new SelectListItem
                {
                    Value = phong.Loaiphong,
                    Text = phong.Loaiphong
                });
            }
            
            return Json(loaiphongListItem);
        }

        public void CreateSelectData()
        {


            List<Dongthietbi> DongtbList = Dongtb.GetAllDongThietBi();
            List<SelectListItem> DongTbSelect = DongtbList.Select(tb => new SelectListItem
            {
                Value = tb.Madongtb.ToString(),
                Text = tb.Tendongtb 
            }).ToList();
            ViewBag.DongTbList = DongTbSelect;

            //var jsonThietbi = JsonConvert.SerializeObject(tbList);
            ViewBag.DongTbFull = DongtbList;


            List<Thietbi> tb = thietbi.GetTBList();
            List<string> stringState = tb.Select(c => c.Trangthai).Distinct().ToList();
            List<SelectListItem> stateSelect = stringState.Select(x => new SelectListItem
            {
                Value = x,
                Text = x
            }).ToList();
            ViewBag.stateSelect = stateSelect;

            List<Coso> csList = coso.GetCoSoList();
            List<SelectListItem> csListItem = csList.Select(cs => new SelectListItem { Value = cs.Macs.ToString(), Text = cs.Tencs }).ToList();
            ViewBag.CoSoList = csListItem;


            List<Phong> roomList = pRepo.GetPhongList();
            List<SelectListItem> phongListItem = roomList.Select(p => new SelectListItem
            {
                Value = p.Map.ToString(),
                Text = p.Map.ToString() + "-" + p.Tenphong + "," + p.Macs.ToString(),
            }).ToList();
            List<string> stringLoaiPhong = roomList.Select(p=>p.Loaiphong).Distinct().ToList();
            List<SelectListItem> roomType = stringLoaiPhong.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            }).ToList();
            ViewBag.PhongList = phongListItem;
            ViewBag.PhongFull = roomList;
            ViewBag.roomType = roomType;
        }
    }
}
