using Microsoft.AspNetCore.Mvc;
using ThietBiDienTu_2.Controllers;
using ThietBiDienTu_2.Models.Authentication;
using ThietBiDienTu_2.Models.ViewModels;
using ThietBiDienTu_2.Models;
using X.PagedList;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[AuthenticationCustomer]
public class HistroyController : Controller
{
    private readonly ToolDbContext _dataContext;
    private readonly ILogger<HistroyController> _logger;
    IHttpContextAccessor contextAcc;
    public HistroyController(ILogger<HistroyController> logger, ToolDbContext context, IHttpContextAccessor contextAcc)
    {
        _logger = logger;
        _dataContext = context;
        this.contextAcc = contextAcc;
    }

    public IActionResult Index(int? page, string? trangThai, DateTime? from, DateTime? to, string? searchMapm)
    {
        int pageSize = 10; // Số lượng mục trong mỗi trang
        int pageNumber = page ?? 1; // Trang mặc định
        // Lấy mã số sinh viên từ Session
        int masv = HttpContext.Session.GetInt32("UserName") ?? 0;

        // Lọc danh sách phiếu mượn chỉ hiển thị của sinh viên hiện tại
        var phieuMuonList = _dataContext.Phieumuons.Where(x => x.Masv == masv).OrderBy(x=> x.Trangthai).ThenByDescending(x => x.Mapm).ToList();

        if (from.HasValue)
        {
            ViewBag.fromDate = from;
            phieuMuonList = phieuMuonList.Where(x => x.Ngaymuon >= from).ToList();
        }
        if (to.HasValue)
        {

            phieuMuonList = phieuMuonList.Where(x => x.Ngaymuon.Date <= to.Value.Date).ToList();
        }

        if (!string.IsNullOrEmpty(trangThai) && trangThai != "-1")
        {
            phieuMuonList = phieuMuonList.Where(x => x.Trangthai.ToString() == trangThai).ToList();
            ViewBag.trangThai = trangThai;
        }
        // Lọc theo mã phiếu mượn
        if (!string.IsNullOrEmpty(searchMapm))
        {
            phieuMuonList = phieuMuonList.Where(x => x.Masv.ToString().Contains(searchMapm)
                                || x.Mapm.ToString().Contains(searchMapm)).ToList();
            ViewBag.searchMapm = searchMapm;
        }


        // Chuyển đổi danh sách thành IEnumerable trước khi sử dụng ToPagedList()
        var pagedList = phieuMuonList.Select(x => new Phieumuon
        {
            Mapm = x.Mapm,
            Ngaymuon = x.Ngaymuon,
            Ngaylap = x.Ngaylap,
            Manv = x.Manv,
            Masv = x.Masv,
            Trangthai = x.Trangthai,
            Lydomuon = x.Lydomuon,
            MasvNavigation = _dataContext.Sinhviens.FirstOrDefault(y => y.Masv == x.Masv),
            ManvNavigation = _dataContext.Nhanviens.FirstOrDefault(y => y.Manv == x.Manv)
        }).AsEnumerable(); // Chuyển đổi thành IEnumerable

        // Sử dụng phương thức Count() để đếm số lượng phần tử trong danh sách
        for (int i = 0; i < pagedList.Count(); ++i)
        {
            pagedList.ElementAt(i).MasvNavigation.MakhoaNavigation = _dataContext.Khoas.FirstOrDefault(x => pagedList.ElementAt(i).MasvNavigation.Makhoa == x.Makhoa);
            pagedList.ElementAt(i).MasvNavigation.ManganhNavigation = _dataContext.Nganhs.FirstOrDefault(x => pagedList.ElementAt(i).MasvNavigation.Manganh == x.Manganh);
        }

        if (IsAjaxRequest())
        {
            PagedList<Phieumuon>  pm = new PagedList<Phieumuon>(phieuMuonList, pageNumber, pageSize);
            return PartialView("PartialViewPhieuMuon", pm);
        }
            return View(pagedList.ToPagedList(pageNumber, pageSize));
    }

    [HttpGet]
    public async Task<IActionResult> ChiTietPhieuMuon(int id)
    {
        // Lấy mã số sinh viên từ Session
        int masv = HttpContext.Session.GetInt32("UserName") ?? 0;

        // Truy vấn dữ liệu để lấy thông tin chi tiết của phiếu mượn
        Phieumuon pm = _dataContext.Phieumuons.FirstOrDefault(x => x.Mapm == id && x.Masv == masv);
        if (pm == null)
        {
            // Xử lý trường hợp không tìm thấy Phieumuon có Mapm tương ứng với id hoặc không phải của sinh viên hiện tại
            return NotFound();
        }

        // Truy vấn dữ liệu để lấy thông tin sinh viên liên quan đến phiếu mượn
        Sinhvien sv = _dataContext.Sinhviens.FirstOrDefault(x => x.Masv == pm.Masv);
        if (sv == null)
        {
            // Xử lý trường hợp không tìm thấy Sinhvien có Masv tương ứng với Masv trong Phieumuon
            return NotFound();
        }

        // Tạo đối tượng HomeViewModel và truyền thông tin của phiếu mượn vào
        PhieuMuonViewModel pmView = new PhieuMuonViewModel
        {
            Manv = pm.Manv,
            Masv = pm.Masv,
            Mapm = pm.Mapm,
            Tensv = sv.Tensv,
            TenKhoa = _dataContext.Khoas.FirstOrDefault(x => x.Makhoa == sv.Makhoa)?.Tenkhoa,
            TenNganh = _dataContext.Nganhs.FirstOrDefault(x => x.Manganh == sv.Manganh)?.Tennganh,
            Trangthai = pm.Trangthai,
            Lydomuon = pm.Lydomuon,
            LydoTuChoi = pm.LydoTuChoi,
            LydoHuy = pm.LydoHuy,
            Ngaylap = pm.Ngaylap,
            NgayDat = pm.Ngaymuon
        };

        // Lấy danh sách chi tiết phiếu mượn từ database
        List<Chitietphieumuon> ctpm = _dataContext.Chitietphieumuons.Where(x => x.Mapm == id)
                                            .Include(x=>x.MatbNavigation).ToList();

        // Tạo danh sách chứa thông tin chi tiết phiếu mượn
        List<ChitietPhieuMuonViewModel> ctpmView = new List<ChitietPhieuMuonViewModel>();

        // Lặp qua danh sách chi tiết phiếu mượn và thêm vào danh sách ctpmView
        foreach (Chitietphieumuon ctpmItem in ctpm)
        {
            var thietbi = _dataContext.Thietbis.FirstOrDefault(x => x.Matb == ctpmItem.Matb);
            // Tạo đối tượng ChitietPhieuMuonViewModel
            ChitietPhieuMuonViewModel ctpmViewTemp = ctpmView.FirstOrDefault(x => x.Madongtb == thietbi.Madongtb);
            if (ctpmViewTemp != null)
            {
                ctpmViewTemp.Matb.Add(thietbi.Matb);
                ctpmViewTemp.Soluong += 1;
                ctpmViewTemp.Seri.Add(thietbi.Seri);
                ctpmViewTemp.Ngaytra.Add(ctpmItem.Ngaytra ?? DateTime.Parse("2004-11-01"));
            }
            else
            {
                ChitietPhieuMuonViewModel ctpmViewItem = new ChitietPhieuMuonViewModel
                {
                    Madongtb = ctpmItem.MatbNavigation.Madongtb,
                    Tendongthietbi = _dataContext.Dongthietbis.FirstOrDefault(x => x.Madongtb == ctpmItem.MatbNavigation.Madongtb)?.Tendongtb,
                    Soluong = 1, // Khởi tạo số lượng là 1
                    Hinhanh = _dataContext.Dongthietbis.FirstOrDefault(x => x.Madongtb == ctpmItem.MatbNavigation.Madongtb)?.Hinhanh,
                    Seri = new List<string> { thietbi.Seri }, // Khởi tạo danh sách Seri
                    Matb = new List<int> { ctpmItem.Matb }, // Khởi tạo danh sách Matb
                    Ngaytra = new List<DateTime> { ctpmItem.Ngaytra ?? DateTime.Parse("2004-11-01") }, // Khởi tạo danh sách Ngaytra
                    check = new List<bool> { ctpmItem.Ngaytra.HasValue && ctpmItem.Ngaytra.Value.Year > 2010 } // Khởi tạo danh sách Check
                };

                // Thêm vào danh sách ctpmView
                ctpmView.Add(ctpmViewItem);
            }
            
        }

        // Gán danh sách chi tiết phiếu mượn vào HomeViewModel
        pmView.ctpmView = ctpmView;

        // Trả về view và truyền đối tượng HomeViewModel vào để hiển thị thông tin
        return View(pmView);
    }
   

    public IActionResult HuyPhieuMuon(int id, string lydoHuy)
    {
        // Lấy thông tin phiếu mượn từ cơ sở dữ liệu
        var phieuMuon = _dataContext.Phieumuons.FirstOrDefault(x => x.Mapm == id);

        // Kiểm tra xem phiếu mượn có tồn tại không
        if (phieuMuon == null)
        {
            return NotFound();
        }

        // Cập nhật trạng thái và lý do hủy của phiếu mượn
        phieuMuon.Trangthai = 5; // Trạng thái hủy
        phieuMuon.LydoHuy = lydoHuy; // Lý do hủy

        // Lưu thay đổi vào cơ sở dữ liệu
        _dataContext.SaveChanges();

        // Kiểm tra xem yêu cầu có phải là AJAX request hay không
        if (IsAjaxRequest())
        {
            // Nếu là AJAX request, trả về kết quả JSON
            return Json(new { success = true });
        }
        else
        {
            // Nếu không phải là AJAX request, chuyển hướng về trang chính sau khi hủy
            return RedirectToAction("Index");
        }
    }
    private bool IsAjaxRequest()
    {
        var request = contextAcc.HttpContext.Request;
        return request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}
