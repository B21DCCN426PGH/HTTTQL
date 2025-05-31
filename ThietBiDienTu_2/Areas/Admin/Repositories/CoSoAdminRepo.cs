using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.Repositories
{
    public class CoSoAdminRepo : ICoSoAdmin
    {
        ToolDbContext toolDbContext;
        public CoSoAdminRepo(ToolDbContext _toolDbContext)
        {
            toolDbContext = _toolDbContext;
        }

        public List<Coso> GetCoSoList()
        {
            List<Coso> cosoList = toolDbContext.Cosos.ToList();
            return cosoList;
        }
    }
}
