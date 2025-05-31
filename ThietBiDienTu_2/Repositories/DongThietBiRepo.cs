using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThietBiDienTu_2.InterfaceRepositories;
using ThietBiDienTu_2.Models.ViewModels;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Repositories
{
    public class DongThietBiRepo : IDongThietBi
    {
        ToolDbContext context;
        public DongThietBiRepo(ToolDbContext _context)
        {
            this.context = _context;
        }

        public List<Dongthietbi> GetAllDongThietBi()
        {
            List<Dongthietbi> tbList = context.Dongthietbis.ToList();
            return tbList;
        }

        public bool CountSlFullOrNot(int MaDongTb)
        {
            int Soluongtb = context.Dongthietbis.FirstOrDefault(x => x.Madongtb == MaDongTb).Soluong;
            int count = context.Thietbis.Where(x => x.Madongtb == MaDongTb).Count();
            return count >= Soluongtb ? true : false;
        }

        public void Updatedtb(Dongthietbi dongthietbi)
        {
           //Dongthietbi dtb = context.Dongthietbis.Find(dongthietbi.Madongtb);
            //dtb.Hinhanh = dongthietbi.Hinhanh;
            //dtb.Soluong = dongthietbi.Soluong;
            //dtb.Mota = dongthietbi.Mota;
            context.Update(dongthietbi);
            context.SaveChanges();
        }

        public Dongthietbi GetDtbById(int id)
        {
            Dongthietbi dtb = context.Dongthietbis.Find(id);
            return dtb;
        }

        public List<DongTbAndAmount> DongTbAndAmountTbInDay(DateTime Ngaymuon)
        {
            List<Chitietphieumuon> allCtpmInDay = context.Chitietphieumuons
                                                          .Include(x => x.MapmNavigation)
                                                          .Where(x => x.MapmNavigation.Ngaymuon == Ngaymuon && x.Ngaytra==null
                                                          && x.MapmNavigation.Trangthai <4)
                                                          .ToList();

            List<int> madongtbdamuon = allCtpmInDay.Select(x => x.Matb).ToList();

            List<Dongthietbi> dongtb = context.Dongthietbis.Select(x => new Dongthietbi
            {
                Tendongtb = x.Tendongtb,
                Hinhanh = x.Hinhanh,
                Mota = x.Mota,
                Madongtb = x.Madongtb,
                Soluong = x.Soluong,
                Thietbis = context.Thietbis.Where(a=>a.Madongtb == x.Madongtb && a.Trangthai =="Sẵn sàng" 
                                            && !madongtbdamuon.Contains(a.Matb)).ToList(),
            }).ToList();
            List<DongTbAndAmount> dongtbAmount = dongtb.Select(x => new DongTbAndAmount
            {
                madongtb = x.Madongtb,
                tendongtb = x.Tendongtb,
                hinhanh = x.Hinhanh,
                amount = x.Thietbis.Count
            }).
            Where(x=>x.amount >0).ToList();
            
            return dongtbAmount;
        }
    }
}
