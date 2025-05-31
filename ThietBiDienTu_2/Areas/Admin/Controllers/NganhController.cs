using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.Controllers
{
    [Area("admin")]
    public class NganhController : Controller
    {
        private readonly IHttpContextAccessor contextAccess;
        private readonly ToolDbContext _context;

        public NganhController(ToolDbContext context, IHttpContextAccessor contextAccess)
        {
            _context = context;
            this.contextAccess = contextAccess;
        }

        public IActionResult Index(string? searchString, int? page)
        {
            var nganhs = string.IsNullOrEmpty(searchString)
                ? _context.Nganhs.ToList()
                : _context.Nganhs.Where(n => n.Tennganh.Contains(searchString)).ToList();
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var pagedNganhs = nganhs.OrderByDescending(x=>x.Manganh).ToPagedList(pageNumber, pageSize);
            if (IsAjaxRequest())
            {
                return PartialView("PartialViewNganh", pagedNganhs);
            }
            return View(pagedNganhs);
        }

        private bool IsAjaxRequest()
        {
            var request = contextAccess.HttpContext.Request;
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
        
        public IActionResult Details(int id)
        {
            var nganh = _context.Nganhs.FirstOrDefault(m => m.Manganh == id);
            if (nganh == null)
            {
                return NotFound();
            }

            return View(nganh);
        }
        // NganhController

        public IActionResult Create(string tenNganh)
        {

            Nganh nganh = new Nganh
            {
                Tennganh = tenNganh
            };
            _context.Nganhs.Add(nganh);
            _context.SaveChanges();
            List<Nganh> nganhList = _context.Nganhs.OrderByDescending(x => x.Manganh).ToList();
            PagedList<Nganh> nganhPage = new PagedList<Nganh>(nganhList, 1, 5);
            return PartialView("PartialViewNganh",nganhPage);
        }

        public IActionResult Edit(int id)
        {
            Nganh nganh = _context.Nganhs.FirstOrDefault(x=>x.Manganh==id);
            
            return Json(nganh);
        }

        [HttpPost]
        public IActionResult Edit(int manganh,string tennganh,string? searchString,int? page)
        {

            Nganh nganh = _context.Nganhs.FirstOrDefault(x => x.Manganh == manganh);
            nganh.Tennganh = tennganh;
            _context.Nganhs.Update(nganh);
            _context.SaveChanges();

            List<Nganh> nganhList = _context.Nganhs.OrderByDescending(x => x.Manganh).ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                nganhList = nganhList.Where(x => x.Tennganh.ToLower().Contains(searchString.ToLower())).ToList();
            }
            PagedList<Nganh> nganhPage = new PagedList<Nganh>(nganhList, page??1, 5);
            return PartialView("PartialViewNganh", nganhPage);
        }

        public IActionResult Delete(int id)
        {
            var nganh = _context.Nganhs.Find(id);
            bool hasSv = _context.Sinhviens.Any(x => x.Manganh == id);
            if (hasSv)
            {
                TempData["ErrorMessage"] = "Thất bại! Ngành hiện đang có sinh viên";
                return RedirectToAction(nameof(Index));
            }

            _context.Nganhs.Remove(nganh);
            TempData["Action"] = "Xóa thành công";
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool NganhExists(int id)
        {
            return _context.Nganhs.Any(e => e.Manganh == id);
        }

    }
}
