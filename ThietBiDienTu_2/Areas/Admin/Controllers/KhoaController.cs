using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.Controllers
{
    [Area("admin")]
    public class KhoaController : Controller
    {
        private readonly ToolDbContext _context;

        public KhoaController(ToolDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;

            var pageNumber = page ?? 1;
            var pageSize = 5;
            var khoas = string.IsNullOrEmpty(searchString)
          ? await _context.Khoas.OrderByDescending(x => x.Makhoa).ToPagedListAsync(pageNumber, pageSize)
          : await _context.Khoas.Where(k => k.Tenkhoa.Contains(searchString)).ToPagedListAsync(pageNumber, pageSize);


            if (IsAjaxRequest())
            {
                return PartialView("PartialViewKhoa", khoas);
            }
            return View(khoas);
        }

        private bool IsAjaxRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }


        public IActionResult Details(int id)
        {
            var khoa = _context.Khoas.FirstOrDefault(m => m.Makhoa == id);
            if (khoa == null)
            {
                return NotFound();
            }

            return View(khoa);
        }

        public IActionResult Create(string tenKhoa)
        {

            Khoa khoa = new Khoa
            {
                Tenkhoa = tenKhoa.ToUpper()
            };
            _context.Add(khoa);
            _context.SaveChanges();

            List<Khoa> khoas = _context.Khoas.OrderByDescending(x => x.Makhoa).ToList();
            PagedList<Khoa> pagedKhoas = new PagedList<Khoa>(khoas, 1, 5);
            return PartialView("PartialViewKhoa", pagedKhoas);
        }

        public IActionResult Edit(int id)
        {
            Khoa khoa = _context.Khoas.FirstOrDefault(x=>x.Makhoa == id);
            return Json(khoa);
        }

        [HttpPost]
        public IActionResult Edit(int makhoa,string tenkhoa,string? searchString,int? page)
        {
            Khoa khoa = _context.Khoas.FirstOrDefault(x => x.Makhoa == makhoa);
            khoa.Tenkhoa = tenkhoa.ToUpper();

            _context.Update(khoa);
            _context.SaveChanges();

            List<Khoa> khoas = _context.Khoas.OrderByDescending(x => x.Makhoa).ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                khoas = khoas.Where(x => x.Tenkhoa.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            PagedList<Khoa> pagedKhoas = new PagedList<Khoa>(khoas, page ?? 1, 5);
            return PartialView("PartialViewKhoa", pagedKhoas);

        }

        public IActionResult Delete(int id)
        {
            var khoa = _context.Khoas.Find(id);
            bool hasSv = _context.Sinhviens.Any(x => x.Makhoa == id);
            if(hasSv)
            {
                TempData["ErrorMessage"] = "Thất bại! Khóa hiện đang có sinh viên";
                return RedirectToAction(nameof(Index));
            }

            _context.Khoas.Remove(khoa);
            TempData["Action"] = "Xóa thành công";
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool KhoaExists(int id)
        {
            return _context.Khoas.Any(e => e.Makhoa == id);
        }

    }
}