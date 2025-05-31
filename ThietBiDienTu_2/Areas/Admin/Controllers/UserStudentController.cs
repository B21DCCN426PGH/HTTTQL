using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Areas.Admin.ViewModels;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [AuthenticationManager]
    public class UserStudentController : Controller
    {
        private readonly IHttpContextAccessor contextAccess;
        private readonly ISinhvienAdmin Svrepo;

        ToolDbContext _context;
        public UserStudentController(ToolDbContext context, IHttpContextAccessor contextAccess, ISinhvienAdmin Svrepo)
        {
            _context = context;
            this.contextAccess = contextAccess;
            this.Svrepo = Svrepo;
        }

        public IActionResult Index(int? page, string? searchStringController, int? filterKhoa, int? filterNganh)
        {
            CreateData();
            var Sinhviens = Svrepo.GetSinhvienList;
            int pageSize = 5;
            int pageNumber = page ?? 1;
            var svList = _context.Sinhviens.Select(x => new Sinhvien
            {
                Masv = x.Masv,
                Tensv = x.Tensv,
                Makhoa = x.Makhoa,
                Manganh = x.Manganh,
                Email = x.Email,
                Sdt = x.Sdt,
                Ngaysinh = x.Ngaysinh,
                Gioitinh = x.Gioitinh,
                MakhoaNavigation = _context.Khoas.FirstOrDefault(a => a.Makhoa == x.Makhoa),
                ManganhNavigation = _context.Nganhs.FirstOrDefault(a => a.Manganh == x.Makhoa),
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchStringController))
            {
           
                svList = svList.Where(x =>
                    x.Tensv != null && (x.Tensv.ToLower().Contains(searchStringController.ToLower()) || x.Masv.ToString().Contains(searchStringController)) 
                ); 
                ViewBag.searchStringController = searchStringController;
            }
            if (filterKhoa != null && filterKhoa != 0)
            {
                svList = svList.Where(x => x.Makhoa == filterKhoa);
                ViewBag.filterKhoa = filterKhoa;
            }
            if (filterNganh != null && filterNganh != 0)
            {
                svList = svList.Where(x => x.Manganh == filterNganh);
                ViewBag.filterNganh = filterNganh;
            }

            svList = svList.OrderBy(x => x.Tensv);
            var sv = new PagedList<Sinhvien>(svList, pageNumber, pageSize);
            if (IsAjaxRequest())
            {
                return PartialView("PartialViewSv", sv);
            }
            return View(sv);
        }

        private bool IsAjaxRequest()
        {
            var request = contextAccess.HttpContext.Request;
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
        public IActionResult Create()
        {
            CreateData();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Sinhvien sv)
        {
            CreateData();

            if (!ModelState.IsValid)
            {
                // Print out model state errors
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        // You can print out the error message or handle it as you like
                        string errorMessage = error.ErrorMessage;
                        string exceptionMessage = error.Exception?.Message;

                        // Print error message to the console or log it
                        Debug.WriteLine($"Error: {errorMessage}");

                        // Handle exception message if there's an exception associated with the error
                        if (exceptionMessage != null)
                        {
                            Debug.WriteLine($"Exception: {exceptionMessage}");
                        }
                    }
                }
            
            return View(sv);
            }
            else if (_context.Sinhviens.Any(x => x.Masv == sv.Masv))
            {
                ModelState.AddModelError("Masv", "Mã sinh viên đã tồn tại");
                return View(sv);
            }
            else
            {
                Taikhoan newAcc = new Taikhoan
                {
                    Matk = sv.Masv,
                    Matkhau = "123456",
                    Loaitk = 0
                };
                _context.Taikhoans.Add(newAcc);

                sv.MakhoaNavigation = null;
                sv.ManganhNavigation = null;
                sv.MasvNavigation = newAcc;
                _context.Sinhviens.Add(sv);
                _context.SaveChanges();
                TempData["Action"] = "Tạo thành công";
                return RedirectToAction("Index");
            }
            
        }

        public IActionResult Update(int id)
        {
            CreateData();
            Sinhvien _sv = _context.Sinhviens.FirstOrDefault(x => x.Masv == id);
            Taikhoan _acc = _context.Taikhoans.FirstOrDefault(x => x.Matk == id);
            StuAcc stuacc = new StuAcc
            {
                sv = _sv,
                acc = _acc
            };
            return View(stuacc);
        }

        [HttpPost]
        public IActionResult Update(StuAcc stu_acc)
        {
            CreateData();
            Debug.WriteLine("In update post");
            if (!ModelState.IsValid)
            {
                return View(stu_acc);
            }
            else if (_context.Sinhviens.Any(x => x.Masv == stu_acc.sv.Masv && x.Masv != stu_acc.acc.Matk))
            {
                ModelState.AddModelError("sv.Masv", "Mã sinh viên đã tồn tại");
                return View(stu_acc);
            }
            else
            {
                
                Taikhoan tkTemp = new Taikhoan
                {
                    Matk = -1,
                    Matkhau = "123456",
                    Loaitk =0 
                };
                _context.Taikhoans.Add(tkTemp);


                Sinhvien svTemp = new Sinhvien
                {
                    Masv = -1,
                    Tensv = "123",
                    Ngaysinh = DateTime.Now,
                    Gioitinh = "Nam",
                    Email = "abc.com",
                    Sdt = "112",
                    Makhoa = stu_acc.sv.Makhoa,
                    Manganh = stu_acc.sv.Manganh,
                };
                _context.Sinhviens.Add(svTemp);
                _context.SaveChanges();

                List<Phieumuon> pmSv = _context.Phieumuons.Where(x => x.Masv == stu_acc.acc.Matk).ToList();
                for(int i = 0; i < pmSv.Count(); ++i)
                {
                    pmSv[i].Masv = svTemp.Masv;
                }

                _context.Phieumuons.UpdateRange(pmSv);
                _context.SaveChanges();

                //Update method doesn't track the change of the primary key 
                //So need to remove then add new one
                Taikhoan acc = _context.Taikhoans.FirstOrDefault(x => x.Matk == stu_acc.acc.Matk);
                Sinhvien stu = _context.Sinhviens.FirstOrDefault(x => x.Masv == stu_acc.acc.Matk);

                //Remove stu first cuz in database stu has foreign key to acc
                _context.Sinhviens.Remove(stu);
                _context.Taikhoans.Remove(acc);   

                stu_acc.acc.Matk = stu_acc.sv.Masv;
                stu_acc.acc.Matkhau = stu_acc.acc.Matkhau;
                stu_acc.sv.MasvNavigation = stu_acc.acc;
                stu_acc.sv.ManganhNavigation = null;
                stu_acc.sv.MakhoaNavigation = null;
                _context.Taikhoans.Add(stu_acc.acc);
                _context.Sinhviens.Add(stu_acc.sv);
                _context.SaveChanges();

                for (int i = 0; i < pmSv.Count(); ++i)
                {
                    pmSv[i].Masv = stu_acc.sv.Masv;
                }

                _context.Phieumuons.UpdateRange(pmSv);
                _context.SaveChanges();

                _context.Sinhviens.Remove(svTemp);
                _context.Taikhoans.Remove(tkTemp);
                _context.SaveChanges();

                TempData["Action"] = "Cập nhật thành công";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Delete(int id)
        {
            Phieumuon pm = _context.Phieumuons.FirstOrDefault(x => x.Masv == id);
            if (pm == null)
            {
                Sinhvien sv = _context.Sinhviens.Find(id);
                Taikhoan acc = _context.Taikhoans.Find(id);

                //Remove stu first cuz in database stu has foreign key to acc
                _context.Sinhviens.Remove(sv);
                _context.Taikhoans.Remove(acc);
                _context.SaveChanges();
            }
            else
            {
                TempData["Fail"] = "Nhân viên không thể xóa";
            }
            
            return RedirectToAction("Index");
        }

        public void CreateData()
        {
            List<SelectListItem> khoaType = _context.Khoas.Select(x => new SelectListItem
            {
                Value = x.Makhoa.ToString(),
                Text = x.Tenkhoa
            }).ToList();
            ViewBag.khoaType = khoaType;
            List<SelectListItem> nganhType = _context.Nganhs.Select(x => new SelectListItem
            {
                Value = x.Manganh.ToString(),
                Text = x.Tennganh
            }).ToList();
            ViewBag.nganhType = nganhType;
        }
    }
}
