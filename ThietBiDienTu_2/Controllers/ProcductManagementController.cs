using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using ThietBiDienTu_2.Models.ViewModels;
using ThietBiDienTu_2.Repository;

namespace ThietBiDienTu_2.Controllers
{
    [AuthenticationCustomer]
    public class ProcductManagementController : Controller
    {
        private readonly ToolDbContext _dataContext;
        private readonly ILogger<ProcductManagementController> _logger; IHttpContextAccessor contextAcc;

        public ProcductManagementController(ILogger<ProcductManagementController> logger, ToolDbContext context, IHttpContextAccessor contextAcc)
        {
            _logger = logger;
            _dataContext = context;
            this.contextAcc = contextAcc;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel();

            // Kiểm tra xem session NgayDat có tồn tại không
            string sessionNgayDat = HttpContext.Session.GetString("NgayDat");
            DateTime? NgayDat = null;
            if (!string.IsNullOrEmpty(sessionNgayDat))
            {
                // Chuyển đổi session NgayDat từ chuỗi thành DateTime
                if (DateTime.TryParseExact(sessionNgayDat, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedNgayDat))
                {
                    NgayDat = parsedNgayDat;
                }
            }

            // Lấy danh sách đồng thiết bị từ cơ sở dữ liệu
            List<Dongthietbi> displayList = new List<Dongthietbi>();

            if (NgayDat != null)
            {
                // Lọc ra các mã thiết bị đã được mượn vào ngày đó
                List<string> maThietBiDaMuon = _dataContext.Chitietphieumuons
                    .Where(ct => ct.MapmNavigation.Ngaymuon.Date == NgayDat.Value.Date)
                    .Select(ct => ct.Matb.ToString())
                    .ToList();

                // Loại bỏ các thiết bị đã được mượn khỏi danh sách đồng thiết bị
                displayList = _dataContext.Dongthietbis.Select(x => new Dongthietbi
                {
                    Madongtb = x.Madongtb,
                    Mota = x.Mota,
                    Soluong = _dataContext.Thietbis.Where(y => y.Madongtb == x.Madongtb && y.Trangthai == "Sẵn sàng" && !maThietBiDaMuon.Contains(y.Matb.ToString())).Count(),
                    Hinhanh = x.Hinhanh,
                    Tendongtb = x.Tendongtb,
                }).Where(x=>x.Soluong>0).ToList();
                List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (CartItemModel item in cart)
                {
                    Dongthietbi dongtb = displayList.FirstOrDefault(x => x.Madongtb == item.Madongtb);
                    dongtb.Soluong -= item.Soluong;
                    if (dongtb.Soluong <= 0)
                    {
                        displayList.Remove(dongtb);
                    }
                }
            }
            
            // Giới hạn số lượng đồng thiết bị hiển thị (ví dụ: 4)
            viewModel.DongThietBiList = displayList.Take(4).ToList();
            viewModel.NgayDat = NgayDat; // Gán ngày đặt vào ViewModel

            return View(viewModel);
        }

        public IActionResult Filter(string? searchString, DateTime? NgayDat, string trangThai)
        {
            var viewModel = new HomeViewModel();

            // Lưu ngày đặt vào session nếu NgayDat được truyền vào
            if (NgayDat != null)
            {
                string sessionNgayDat = HttpContext.Session.GetString("NgayDat") ?? "01-01-1999";
                DateTime ngaydatold = DateTime.ParseExact(sessionNgayDat, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                if(ngaydatold.Date != NgayDat.Value.Date)
                {
                    HttpContext.Session.SetString("NgayDat", NgayDat.Value.ToString("dd-MM-yyyy"));
                    HttpContext.Session.Remove("Cart");
                }
            }
            else
            {
                // Nếu NgayDat không được truyền vào, kiểm tra xem đã có session NgayDat hay chưa
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("NgayDat");
            }

            List<Dongthietbi> displayList = new List<Dongthietbi>();

            if (NgayDat != null)
            {
                // Lọc ra các mã thiết bị đã được mượn vào ngày đó
                List<string> maThietBiDaMuon = _dataContext.Chitietphieumuons
                    .Where(ct => ct.MapmNavigation.Ngaymuon.Date == NgayDat.Value.Date)
                    .Select(ct => ct.Matb.ToString())
                    .ToList();

                // Loại bỏ các thiết bị đã được mượn khỏi danh sách đồng thiết bị
                displayList = _dataContext.Dongthietbis.Select(x => new Dongthietbi
                {
                    Madongtb = x.Madongtb,
                    Mota = x.Mota,
                    Soluong = _dataContext.Thietbis.Where(y => y.Madongtb == x.Madongtb && y.Trangthai == "Sẵn sàng" && !maThietBiDaMuon.Contains(y.Matb.ToString())).Count(),
                    Hinhanh = x.Hinhanh,
                    Tendongtb = x.Tendongtb,
                }).Where(x=>x.Soluong>0).ToList();
            }
            
            viewModel.NgayDat = NgayDat; // Gán ngày đặt vào ViewModel
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            foreach (CartItemModel item in cart)
            {
                Dongthietbi dongtb = displayList.FirstOrDefault(x => x.Madongtb == item.Madongtb);
                dongtb.Soluong -= item.Soluong;
                if(dongtb.Soluong <= 0)
                {
                    displayList.Remove(dongtb);
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                displayList = displayList.Where(x => x.Tendongtb.ToLower().Contains(searchString.ToLower())).ToList();
            }

            // Kiểm tra trạng thái để trả về danh sách đồng thiết bị phù hợp
            if (trangThai == "Sẵn sàng")
            {
                // Trả về tất cả các mục
                viewModel.DongThietBiList = displayList;
                ViewBag.XemThem = "Sẵn sàng";
            }
            else
            {
                // Giới hạn số lượng đồng thiết bị hiển thị (ví dụ: 4)
                viewModel.DongThietBiList = displayList.Take(4).ToList();
            }
            
            // Trả về view chính khi là request không phải AJAX
            return PartialView("_PartialShowProduct", viewModel);
        }

        bool IsAjaxRequest()
        {
            var request = contextAcc.HttpContext.Request;
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
