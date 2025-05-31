using Microsoft.EntityFrameworkCore;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Areas.Admin.ViewModels;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.Repositories
{
    public class PhieuSuaAdminRepo : IPhieuSuaAdmin
    {
        ToolDbContext context;
        public PhieuSuaAdminRepo(ToolDbContext context)
        {
            this.context = context;
        }

        public bool TbHasPhieuSua(int matb)
        {
            return context.Chitietphieusuas.FirstOrDefault(x => x.Matb == matb) != null;
        }

        public List<Phieusua> GetAllPs()
        {
            return context.Phieusuas.ToList();
        }

       public int CreatePs(List<TbFixAndCheck> tbChoosenList, decimal tongchiphi)
        {
            DateTime today =  DateTime.Now;
            Phieusua ps = new Phieusua()
            {
                Ngaylap = today,
                Trangthai = 0,
                Tongchiphi = tongchiphi,
            };
            context.Phieusuas.Add(ps);
            context.SaveChanges();

            foreach (TbFixAndCheck tbFix in tbChoosenList)
            {
                Chitietphieusua ctps = new Chitietphieusua
                {
                    Maps = ps.Maps,
                    Matb = tbFix.Matb,
                    Chiphi = tbFix.Chiphi,
                    Mota = tbFix.Mota,
                };
                context.Chitietphieusuas.Add(ctps);
                context.SaveChanges();

                Thietbi tb = context.Thietbis.Find(tbFix.Matb);
                tb.Trangthai = "Đang sửa";
                context.Thietbis.Update(tb);
                context.SaveChanges();
            }
            
            return ps.Maps;
        }

        public bool Deletepheusua(int maps)
        {
            Phieusua ps = context.Phieusuas.FirstOrDefault(x => x.Maps == maps);
            ps.Chitietphieusuas = context.Chitietphieusuas.Where(x => x.Maps == maps).Include(x => x.MatbNavigation).ToList();
            if(ps.Chitietphieusuas.All(x=>x.Ngayhoanthanh!=null))
            {
                //Because when delete all the device change again to "Dang hu"
                return false;
            }
            foreach(Chitietphieusua ctps in ps.Chitietphieusuas)
            {
                ctps.MatbNavigation.Trangthai = "Đang hư";
                context.Thietbis.Update(ctps.MatbNavigation);
                
            }
            context.SaveChanges();

            context.Chitietphieusuas.RemoveRange(ps.Chitietphieusuas);
            context.SaveChanges();

            context.Phieusuas.Remove(ps);
            context.SaveChanges();
            return true;
        }

        public Phieusua GetPsById(int maps)
        {
            Phieusua ps = context.Phieusuas.Find(maps);
            ps.Chitietphieusuas = context.Chitietphieusuas.Where(x => x.Maps == maps).Include(x => x.MatbNavigation)
                                                            .ThenInclude(x => x.MapNavigation)
                                                            .Include(x => x.MatbNavigation)
                                                            .ThenInclude(x => x.MadongtbNavigation).ToList();

            return ps;
        }

        public void UpdatePhieusua(List<TbFixAndCheck> tbFixCheck, Phieusua ps)
        {
            ps.Trangthai = tbFixCheck.Any(x => x.CheckFix == false) ? 0 : 1;
            ps.Tongchiphi = tbFixCheck.Sum(x => x.Chiphi);

            context.Phieusuas.Update(ps);
            context.SaveChanges();
            DateTime today = DateTime.Now; 
            foreach(TbFixAndCheck tb in tbFixCheck)
            {
                Chitietphieusua ctps = context.Chitietphieusuas.FirstOrDefault(x => x.Matb == tb.Matb && x.Maps == ps.Maps);
                ctps.Chiphi = tb.Chiphi;
                ctps.Mota = tb.Mota;
                if(ctps.Ngayhoanthanh.HasValue ==false && tb.CheckFix == true) {
                    ctps.Ngayhoanthanh = today;
                    
                }
                if(tb.CheckFix == false)
                {
                    ctps.Ngayhoanthanh = null;
                }
                context.Chitietphieusuas.Update(ctps);
                context.SaveChanges();

                Thietbi tbUpdate = context.Thietbis.FirstOrDefault(x => x.Matb == tb.Matb);
                if (tb.CheckFix == true)
                {
                    tbUpdate.Trangthai = "Sẵn sàng";

                }
                else if(tb.CheckFix == false)
                {
                    tbUpdate.Trangthai = "Đang sửa";
                }
                context.Thietbis.Update(tbUpdate);
                context.SaveChanges();
            }
        }
    }
}
