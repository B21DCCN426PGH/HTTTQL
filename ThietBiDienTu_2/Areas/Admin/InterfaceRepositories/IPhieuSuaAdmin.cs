using ThietBiDienTu_2.Areas.Admin.ViewModels;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.InterfaceRepositories
{
    public interface IPhieuSuaAdmin
    {
        bool TbHasPhieuSua(int matb);
        List<Phieusua> GetAllPs();
        int CreatePs(List<TbFixAndCheck> tbChoosenList,decimal tongchiphi);
        bool Deletepheusua(int maps);

        Phieusua GetPsById(int maps);
        void UpdatePhieusua(List<TbFixAndCheck> tbFixCheck, Phieusua ps);
    }
}
