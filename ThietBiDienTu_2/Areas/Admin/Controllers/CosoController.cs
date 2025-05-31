using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using X.PagedList;

namespace ThietBiDienTu_2.Areas.Admin.Controllers
{
    [Area("admin")]
    public class CosoController : Controller
    {
        private readonly ICoSoAdmin csRepo;
        private readonly ToolDbContext _context;

        public CosoController(ToolDbContext context)
        {
            _context = context;
        }

        // GET: Cosos
        public IActionResult Index(string? searchString, int? page)
        {

            var cosos = _context.Cosos.OrderByDescending(x=>x.Macs).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                cosos = cosos.Where(x => x.Tencs.ToLower().Contains(searchString.ToLower()));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var pagedCosos = cosos.ToPagedList(pageNumber, pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("PartialViewCoSo", pagedCosos);
            }

            return View(pagedCosos);
        }


        private bool IsAjaxRequest()
        {
            throw new NotImplementedException();
        }


        public IActionResult Create(string tencs, string diachi)
        {
            Coso cs = new Coso
            {
                Tencs = tencs,
                Diachi = diachi
            };
            _context.Add(cs);
            _context.SaveChanges();

            var cosos = _context.Cosos.OrderByDescending(x => x.Macs).AsQueryable();
            PagedList<Coso> pagedCosos = new PagedList<Coso>(cosos, 1, 5);
            return PartialView("PartialViewCoSo", pagedCosos);
        }

        // GET: Cosos/Edit/5
        public IActionResult Edit(int id)
        {
            Coso coso =_context.Cosos.FirstOrDefault(x=>x.Macs == id);
            return Json(coso);
        }

        // POST: Cosos/Edit/5
        [HttpPost]
        public IActionResult Edit(int macs,string tencs,string diachi,string? searchString,int? page)
        {

            Coso coso = _context.Cosos.FirstOrDefault(x => x.Macs == macs);
            coso.Tencs = tencs;
            coso.Diachi = diachi;
            _context.Update(coso);
             _context.SaveChanges();

            List<Coso> cosos = _context.Cosos.OrderByDescending(x => x.Macs).ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                cosos = cosos.Where(x => x.Tencs.ToLower().Contains(searchString.ToLower())).ToList();
            }
            PagedList<Coso> pagedCosos = new PagedList<Coso>(cosos, page ?? 1, 5);
            return PartialView("PartialViewCoSo", pagedCosos);
        }

        private bool CosoExists(int macs)
        {
            return _context.Cosos.Any(e => e.Macs == macs);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var coso = await _context.Cosos.FindAsync(id);

            // Kiểm tra xem cơ sở có chứa phòng hay không
            var hasRooms = _context.Phongs.Any(p => p.Macs == id);

            if (hasRooms)
            {
                TempData["ErrorMessage"] = "Thất bại! Cơ sở này đang có phòng";
                return RedirectToAction("Index");
            }

            _context.Cosos.Remove(coso);
            TempData["AlertMessage"] = "Đã xóa cơ sở thành công";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
