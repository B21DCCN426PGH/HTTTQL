using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using ThietBiDienTu_2.Migrations;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.Authentication;
using ThietBiDienTu_2.Models.ViewModels;
using ThietBiDienTu_2.Repository;

namespace ThietBiDienTu_2.Controllers
{
    [AuthenticationCustomer]
    public class CartController : Controller
    {
        private readonly ToolDbContext _dataContext;
        public CartController(ToolDbContext _context)
        {
            _dataContext = _context;
        }

        public IActionResult Index()
        {

            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>(); // neu co du lieu thi hien thi con khong se tao moi 1 list 
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Soluong)//Tinh tong


            };

            return View(cartVM);
        }


        public IActionResult Add(int id,string? searchString,string trangThai)
        {
            Dongthietbi dongthietbi = _dataContext.Dongthietbis.FirstOrDefault(x => x.Madongtb == id);
            string sessionNgayDat = HttpContext.Session.GetString("NgayDat");
            DateTime NgayDat = DateTime.ParseExact(sessionNgayDat, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var viewModel = new HomeViewModel();
            viewModel.DongThietBiList = checkQuantity(id).ToList();
            viewModel.NgayDat = NgayDat;

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.FirstOrDefault(c => c.Madongtb == id);

            int soluongkho = viewModel.DongThietBiList.FirstOrDefault(x => x.Madongtb == dongthietbi.Madongtb).Soluong;

            if ((cartItem == null ? 0 : cartItem.Soluong) >= soluongkho)
            {
                ViewBag.FailAdd = "true";
                if (soluongkho > 0)
                {
                    cartItem.Soluong = soluongkho;
                }
                else if(cartItem != null)
                {
                    cart.Remove(cartItem);
                }
            }
            else
            {
                if (cartItem == null)
                {
                    cart.Add(new CartItemModel(dongthietbi));
                    ViewBag.Notification = "true";
                }
                else
                {
                    cartItem.Soluong += 1;
                    ViewBag.Notification = "true";
                }
            }
            HttpContext.Session.SetJson("Cart", cart);


            foreach (CartItemModel item in cart)
            {
                Dongthietbi dongtb = viewModel.DongThietBiList.FirstOrDefault(x => x.Madongtb == item.Madongtb);
                dongtb.Soluong -= item.Soluong;
                if (dongtb.Soluong < 0)
                {
                    viewModel.DongThietBiList.Remove(dongtb);
                }
            }
            viewModel.DongThietBiList = viewModel.DongThietBiList.Where(x => x.Soluong > 0).ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                viewModel.DongThietBiList = viewModel.DongThietBiList.Where(x => x.Tendongtb.ToLower().Contains(searchString.ToLower())).ToList();
            }
            if (trangThai == "Sẵn sàng")
            {
                // Trả về tất cả các mục
                ViewBag.XemThem = "Sẵn sàng";
            }
            else
            {
                // Giới hạn số lượng đồng thiết bị hiển thị (ví dụ: 4)
                viewModel.DongThietBiList = viewModel.DongThietBiList.Take(4).ToList();
            }
            return PartialView("_PartialShowProduct", viewModel);
        }

        public List<Dongthietbi> checkQuantity(int id)
        {
            Dongthietbi dongthietbi = _dataContext.Dongthietbis.FirstOrDefault(x => x.Madongtb == id);
            string sessionNgayDat = HttpContext.Session.GetString("NgayDat");
            DateTime NgayDat = DateTime.ParseExact(sessionNgayDat, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<string> maThietBiDaMuon = _dataContext.Chitietphieumuons
                    .Where(ct => ct.MapmNavigation.Ngaymuon.Date == NgayDat.Date)
                    .Select(ct => ct.Matb.ToString())
                    .ToList();

            List<Dongthietbi> displayList = _dataContext.Dongthietbis.Select(x => new Dongthietbi
            {
                Madongtb= x.Madongtb,
                Mota = x.Mota,
                Soluong = _dataContext.Thietbis.Where(y => y.Madongtb == x.Madongtb && y.Trangthai == "Sẵn sàng" && !maThietBiDaMuon.Contains(y.Matb.ToString())).Count(),
                Hinhanh = x.Hinhanh,
                Tendongtb = x.Tendongtb,
            }).ToList();

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            return displayList;
        }
        public IActionResult Decrease(int id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(c => c.Madongtb == id);
            Dongthietbi dongthietbi = checkQuantity(id).FirstOrDefault(x => x.Madongtb == id);
            int availableQuantity = dongthietbi.Soluong;

            if (cartItem.Soluong - 1 > availableQuantity)
            {
                //TempData[$"Message_{id}"] = "Thiết bị chỉ còn " + availableQuantity + " số lượng";
                TempData["FailAdd"] = "true";
                if (availableQuantity > 0)
                {
                    cartItem.Soluong = availableQuantity;
                }
                else
                {
                    cart.Remove(cartItem);
                }
            }
            else
            {
                if (cartItem.Soluong > 1)
                {
                    --cartItem.Soluong;
                }
                else
                {
                    cart.RemoveAll(p => p.Madongtb == id);
                }
            }
            HttpContext.Session.SetJson("Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult Increase(int id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(c => c.Madongtb == id);
            Dongthietbi dongthietbi = checkQuantity(id).FirstOrDefault(x => x.Madongtb == id);
            int availableQuantity = dongthietbi.Soluong;

            if (cartItem.Soluong >= availableQuantity)
            {
                // Lưu thông báo vượt quá số lượng "Sẵn sàng" vào TempData của sản phẩm
                //TempData[$"Message_{id}"] = "Thiết bị chỉ còn " + availableQuantity + " số lượng";
                TempData["FailAdd"] = "true";
                if (availableQuantity > 0)
                {
                    cartItem.Soluong = availableQuantity;
                }
                else
                {
                    cart.Remove(cartItem);
                }
            }
            else
            {
                if (cartItem == null)
                {
                    cart.Add(new CartItemModel(dongthietbi));
                }
                else
                {
                    cartItem.Soluong++;
                }
            }

            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int Madongtb)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(x => x.Madongtb == Madongtb);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Clear(string Matb)
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }
        public IActionResult Details()
        {
            if (CheckQuantityAll() == false)
            {
                return RedirectToAction("Index");
            }
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>(); // neu co du lieu thi hien thi con khong se tao moi 1 list 
            string ngayDatString = HttpContext.Session.GetString("NgayDat");
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                Sv = _dataContext.Sinhviens.Find(HttpContext.Session.GetInt32("UserName")),
                Phieumuon = new Phieumuon()
                {
                    Ngaylap = DateTime.Now,
                    Ngaymuon = DateTime.ParseExact(ngayDatString, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                }

            };
            
            return View(cartVM);
        }

        public bool CheckQuantityAll()
        {
            string sessionNgayDat = HttpContext.Session.GetString("NgayDat");
            DateTime NgayDat = DateTime.ParseExact(sessionNgayDat, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<string> maThietBiDaMuon = _dataContext.Chitietphieumuons
                    .Where(ct => ct.MapmNavigation.Ngaymuon.Date == NgayDat.Date)
                    .Select(ct => ct.Matb.ToString())
                    .ToList();

            List<Dongthietbi> displayList = _dataContext.Dongthietbis.Select(x => new Dongthietbi
            {
                Madongtb = x.Madongtb,
                Mota = x.Mota,
                Soluong = _dataContext.Thietbis.Where(y => y.Madongtb == x.Madongtb && y.Trangthai == "Sẵn sàng" && !maThietBiDaMuon.Contains(y.Matb.ToString())).Count(),
                Hinhanh = x.Hinhanh,
                Tendongtb = x.Tendongtb,
            }).ToList();

            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            bool overAmount = false;

            int count = cartItems.Count()-1;
            for (int i= count; i >= 0;--i)
            {
                Dongthietbi dongtb = displayList.FirstOrDefault(x => x.Madongtb == cartItems[i].Madongtb);
                if (cartItems[i].Soluong > dongtb.Soluong)
                {
                    overAmount = true;
                    if (dongtb.Soluong == 0)
                    {
                        cartItems.RemoveAt(i);
                    }
                    else
                    {
                        cartItems[i].Soluong = dongtb.Soluong;
                    }
                }
            }

            if (cartItems.Count() == 0)
            {
                HttpContext.Session.Remove("Cart");
                //Trog giỏ hết thiết bị
                TempData["Notifications"] = "true";
                return false;
            }
            else if(overAmount == true)
            {
                HttpContext.Session.SetJson("Cart", cartItems);
                TempData["Notifications"] = "true";
            }

            return true;
        }

        [HttpPost]
        public IActionResult Details(CartItemViewModel cartVM)
        {
            if (CheckQuantityAll() == false)
            {
                return RedirectToAction("Index");
            }
            string ngayDatString = HttpContext.Session.GetString("NgayDat");
            DateTime ngayMuon = DateTime.ParseExact(ngayDatString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            if (cartVM.Phieumuon.Lydomuon != null) // Kiểm tra xem dữ liệu gửi lên có hợp lệ không
            {
                // Lấy dữ liệu từ form
                string lyDoMuon = cartVM.Phieumuon.Lydomuon;
                int maSv = HttpContext.Session.GetInt32("UserName") ?? 0;

                // Bắt đầu giao dịch
                using (var transaction = _dataContext.Database.BeginTransaction())
                {
                    try
                    {

                        // Tạo đối tượng Phiieumuon và gán dữ liệu từ form
                        Phieumuon phieuMuon = new Phieumuon
                        {
                            Lydomuon = lyDoMuon,
                            Ngaymuon = ngayMuon,
                            Ngaylap = DateTime.Now,
                            Masv = maSv,
                        };

                        // Lưu đối tượng Phieumuon vào cơ sở dữ liệu
                        _dataContext.Phieumuons.Add(phieuMuon);
                        _dataContext.SaveChanges();

                        // Lấy Mã phiếu mượn mới tạo
                        int maPhieuMuon = phieuMuon.Mapm;

                        // Lấy danh sách các thiết bị trong giỏ hàng của sinh viên
                        var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                        List<Chitietphieumuon> ctpmThatDay = _dataContext.Chitietphieumuons.Include(x=>x.MapmNavigation)
                                                                .Where(a=>a.MapmNavigation.Ngaymuon == ngayMuon).ToList();
                        List<string> maThietBiDaMuon = ctpmThatDay.Select(ct => ct.Matb.ToString()).ToList();
                        // Tạo danh sách tạm thời để lưu các phần tử cần loại bỏ
                        var itemsToRemove = new List<CartItemModel>();

                        foreach (var cartItem in cartItems)
                        {
                            // Tìm mã thiết bị có trạng thái "Sẵn sàng" và chưa được đặt trong cùng một ngày
                            List<Thietbi> tbss = _dataContext.Thietbis.Include(a=> a.MapNavigation)
                                .Where(x => x.Madongtb == cartItem.Madongtb && x.Trangthai == "Sẵn sàng" 
                                && !maThietBiDaMuon.Contains(x.Matb.ToString()))
                                .OrderByDescending(z=>z.MapNavigation.Douutien )
                                .ThenByDescending(u=>u.Seri).ToList();
                              
                            
                            
                            

                            if (tbss != null && tbss.Count >=cartItem.Soluong)
                            {
                                // Tạo ChiTietPhieuMuon chỉ với số lượng thiết bị đặt tương ứng
                                for (int i = 0; i < cartItem.Soluong; i++)
                                {
                                    Chitietphieumuon chiTietPhieuMuon = new Chitietphieumuon
                                    {
                                        Mapm = maPhieuMuon,
                                        Matb = tbss.ElementAt(i).Matb,
                                    };
                                    // Lưu đối tượng ChiTietPhieuMuon vào cơ sở dữ liệu
                                    _dataContext.Chitietphieumuons.Add(chiTietPhieuMuon);
                                }

                                // Thêm phần tử vào danh sách cần loại bỏ
                                itemsToRemove.Add(cartItem);
                            }
                        }

                        // Loại bỏ các phần tử đã được xử lý khỏi danh sách giỏ hàng
                        foreach (var item in itemsToRemove)
                        {
                            cartItems.Remove(item);
                        }

                        // Lưu thay đổi vào cơ sở dữ liệu
                        _dataContext.SaveChanges();

                        HttpContext.Session.SetJson("Cart", cartItems);
                        HttpContext.Session.Remove("Cart");

                        // Hoàn thành giao dịch
                        transaction.Commit();

                        TempData["Notifications"] = maPhieuMuon;
                        // Sau khi lưu thành công, bạn có thể thực hiện các hành động khác ở đây, ví dụ: redirect đến trang thành công, hiển thị thông báo, v.v.
                        return RedirectToAction("Index", "Histroy");
                    }
                    catch (Exception ex)
                    {
                        // Nếu có lỗi xảy ra, rollback giao dịch
                        transaction.Rollback();
                        // Xử lý lỗi (ví dụ: log, hiển thị thông báo)
                        Debug.WriteLine("Error: " + ex.Message);
                        // Quay trở lại trang trước đó hoặc hiển thị thông báo lỗi
                        return RedirectToAction("Index", "Histroy");
                    }
                }
            }
            else
            {
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>(); // neu co du lieu thi hien thi con khong se tao moi 1 list 
                cartVM = new()
                {
                    CartItems = cartItems,
                    Sv = _dataContext.Sinhviens.Find(HttpContext.Session.GetInt32("UserName")),
                    Phieumuon = new Phieumuon
                    {
                        Ngaymuon = ngayMuon,
                        Ngaylap = DateTime.Now.Date
                    }
                };
                ModelState.AddModelError("Phieumuon.Lydomuon", "Vui lòng điền lý do mượn");
                return View(cartVM);
            }
        }

    }
}
