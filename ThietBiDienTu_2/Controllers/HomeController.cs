using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using ThietBiDienTu_2.Repository;

namespace ThietBiDienTu_2.Controllers
{
    [AuthenticationCustomer]
    public class HomeController : Controller
    {
        private readonly ToolDbContext _context;
        public HomeController(ToolDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Gọi phương thức tính tổng số lượng từ CartItemModel
            int masv = HttpContext.Session.GetInt32("UserName") ?? 0;
            var student = _context.Sinhviens.FirstOrDefault(s => s.Masv == masv);
            DateTime today = DateTime.Now;
            int amountToday = _context.Phieumuons.Count(x => x.Ngaymuon == today.Date && x.Masv == masv);
            ViewBag.todayCount = amountToday;

            
            int chuatraCount = _context.Phieumuons.Count(p => p.Trangthai == 1 && p.Masv ==masv);
            ViewBag.CTCount = chuatraCount;

            int chuaduyetCount = _context.Phieumuons.Count(p => p.Trangthai == 0 && p.Masv == masv);
            ViewBag.CDCount = chuaduyetCount;

            string studentName = student != null ? student.Tensv : "Sinh viên";

            // Store the student's name in ViewBag
            ViewBag.StudentName = studentName;

            return View();
        }
    }
}


