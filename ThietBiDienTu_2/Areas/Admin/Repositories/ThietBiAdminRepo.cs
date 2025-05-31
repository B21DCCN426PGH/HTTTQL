using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Areas.Admin.ViewModels;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.Repositories
{
    public class ThietBiAdminRepo :IThietBiAdmin
    {
        ToolDbContext context;
        public ThietBiAdminRepo(ToolDbContext _context)
        {
            context = _context;
        }

        public List<Thietbi> GetTBList()
        {
            List<Thietbi> tbList = context.Thietbis.Select(x=>new Thietbi
            {
                Matb = x.Matb,
                Seri = x.Seri,
                Map = x.Map,
                Madongtb = x.Madongtb,
                Trangthai = x.Trangthai,
                MapNavigation = context.Phongs.FirstOrDefault(y=>y.Map ==x.Map),               
                MadongtbNavigation = context.Dongthietbis.FirstOrDefault(y=>y.Madongtb == x.Madongtb),
            }).ToList();
              
            return tbList;
        }

        public Thietbi GetTBById(int _Matb)
        {
            Thietbi tb = context.Thietbis.FirstOrDefault(x => x.Matb == _Matb);
            if (tb != null)
            {
                tb.MapNavigation = context.Phongs.FirstOrDefault(y => y.Map == tb.Map);
                tb.MadongtbNavigation = context.Dongthietbis.FirstOrDefault(y => y.Madongtb == tb.Madongtb);
            }
            

            return tb;
        }

        public Thietbi CheckSeriExist(string seri, int maDongTb, string oldSeri)
        {
            Thietbi tb = context.Thietbis.FirstOrDefault(x => x.Seri == seri && x.Seri != oldSeri && x.Madongtb == maDongTb);
            return tb;
        }
        public void AddTB(Thietbi tb)
        {
            context.Thietbis.Add(tb);
            context.SaveChanges();
        }

        public void UpdateTB(ThietBiViewAdmin ThietBiView)
        {
            Thietbi tb = context.Thietbis.FirstOrDefault(x => x.Matb == ThietBiView.Matb);
            tb.Seri = ThietBiView.Seri;
            tb.Map = ThietBiView.MaP;
            tb.Trangthai = ThietBiView.TrangThai;
            context.Thietbis.Update(tb);
            context.SaveChanges();

        }

        public void DeleteTB(int MaTB)
        {
            Thietbi tb = context.Thietbis.FirstOrDefault(x => x.Matb == MaTB);
            List<Chitietphieumuon> ctpm = context.Chitietphieumuons.Where(x => x.Matb == tb.Matb).ToList();
            context.Chitietphieumuons.RemoveRange(ctpm);

            context.Thietbis.Remove(tb);
            context.SaveChanges();
        }

        public List<Thietbi> GetTbListInThatDay(int matb, DateTime Ngaymuon)
        {
            List<Chitietphieumuon> ctpmThatDay = context.Chitietphieumuons.Include(x => x.MapmNavigation)
                                                        .Where(x => x.MapmNavigation.Ngaymuon == Ngaymuon
                                                         && x.MapmNavigation.Trangthai <4
                                                         &&x.Ngaytra == null).ToList();
            List<int> matbList = ctpmThatDay.Select(x => x.Matb).ToList();
            Thietbi tb = context.Thietbis.FirstOrDefault(x => x.Matb == matb);
            List<Thietbi> tbList = context.Thietbis.Where(x => !matbList.Contains(x.Matb)
                                                    && x.Trangthai == "Sẵn sàng"  && x.Madongtb== tb.Madongtb)
                                                 .Include(x => x.MapNavigation)
                                                 .Include(x => x.MadongtbNavigation)
                                                 .OrderByDescending(x => x.MapNavigation.Douutien)
                                                 .ThenByDescending(x => x.Seri).ToList();

            return tbList;

        }

        public List<TbFixAndCheck> GetTbFixAndCheckList()
        {
            List<TbFixAndCheck> tbFix = context.Thietbis.Where(x => x.Trangthai == "Đang hư" && x.MapNavigation.Loaiphong == "Kho")
                                                    .Include(x => x.MadongtbNavigation)
                                                    .Include(x => x.MapNavigation).Select(x => new TbFixAndCheck
                                                    {
                                                        Matb = x.Matb,
                                                        Seri = x.Seri,
                                                        Hinhanh = x.MadongtbNavigation.Hinhanh,
                                                        Tentb = x.MadongtbNavigation.Tendongtb,
                                                        TenKho = x.MapNavigation.Tenphong,
                                                        Makho = x.Map,
                                                        CheckFix = false,
                                                    }).ToList();

            return tbFix;
        }
    }
}
