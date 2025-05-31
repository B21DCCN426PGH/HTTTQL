using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.Repositories
{
    public class SinhvienAdminRepo : ISinhvienAdmin
    {
        ToolDbContext _toolDbContext;
        public SinhvienAdminRepo(ToolDbContext toolDbContext)
        {
            _toolDbContext = toolDbContext;
        }

        public void Add(Sinhvien sinhvien)
        {
            throw new NotImplementedException();
        }

        public void Delete(Sinhvien sinhvien)
        {
            throw new NotImplementedException();
        }

        public Sinhvien FindSinhvien(int id)
        {
            throw new NotImplementedException();
        }

        public List<Sinhvien> GetSinhvienList()
        {
            throw new NotImplementedException();
        }

        public Sinhvien GetSvById(int id)
        {
            Sinhvien sv = null;
            if(_toolDbContext.Sinhviens.Any(x=>x.Masv == id))
            {
                sv = _toolDbContext.Sinhviens.Find(id);
                sv.MakhoaNavigation = _toolDbContext.Khoas.Find(sv.Makhoa);
                sv.ManganhNavigation = _toolDbContext.Nganhs.Find(sv.Manganh);
            }
            
            return sv;
        }

        public void Update(Sinhvien sinhvien)
        {
            throw new NotImplementedException();
        }
    }
}
