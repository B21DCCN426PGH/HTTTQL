using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.Repositories
{
    public class PhongAdminRepo : IPhongAdmin
    {
        ToolDbContext _toolDbContext;
        public PhongAdminRepo(ToolDbContext toolDbContext)
        {
            _toolDbContext = toolDbContext;
        }

        public List<Phong> GetPhongList()
        {
            List<Phong> pList = _toolDbContext.Phongs.ToList();
            return pList;
        }
        public void Add(Phong phong)
        {
            _toolDbContext.Phongs.Add(phong);
            _toolDbContext.SaveChanges();
        }
        public void Update(Phong phong,string oldMap)
        {
            if(phong.Map != oldMap)
            {
                _toolDbContext.Phongs.Add(phong);
                _toolDbContext.SaveChanges();

                Phong p = _toolDbContext.Phongs.Find(oldMap);
                List<Thietbi> tb = _toolDbContext.Thietbis.Where(x => x.Map == oldMap).ToList();
                tb.ForEach(x => x.Map = phong.Map);
                _toolDbContext.Thietbis.UpdateRange(tb);
                _toolDbContext.SaveChanges();

                _toolDbContext.Phongs.Remove(p);
                _toolDbContext.SaveChanges();
            }
            
            _toolDbContext.Phongs.Update(phong);
            _toolDbContext.SaveChanges();
        }
        public void Delete(string maphong)
        {
            Phong phong = _toolDbContext.Phongs.Find(maphong);
            _toolDbContext.Phongs.Remove(phong);
            _toolDbContext.SaveChanges();
        }
        public Phong FindPhong(string Map)
        {
            var phong = _toolDbContext.Phongs.Find(Map);
            return phong;
        }

        public bool CheckPhongExist(string newMap,string oldMap="")
        {
            return _toolDbContext.Phongs.Any(x => x.Map == newMap && x.Map != oldMap);
        }

        public List<Phong> phongKhoListOfTbList(List<Thietbi> tbList)
        {
            List<Phong> khoList = tbList.Select(x => x.MapNavigation)
                                            .Where(x=>x.Loaiphong=="Kho").Distinct().ToList();
            return khoList;
        }

        public bool CheckIfPhongHasDevices(string id)
        {
            // Kiểm tra trong bảng Thiết bị xem có bản ghi nào tham chiếu đến phòng có mã là id không
            return _toolDbContext.Thietbis.Any(tb => tb.Map == id);
        }

        public List<Phong> GetPhongListByCoso(string macs)
        {
            return _toolDbContext.Phongs.Where(x => x.Macs.ToString() == macs).ToList();
        }
    }
}
