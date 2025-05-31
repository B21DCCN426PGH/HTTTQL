using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.Repositories
{
    public class KhoaAdminRepo :IKhoa
    {
        ToolDbContext context;
        public KhoaAdminRepo(ToolDbContext context)
        {
            this.context = context;
        }

        public List<Khoa> getAllKhoa()
        {
            List<Khoa> k= context.Khoas.ToList();
            return k;
        }
    }
}
