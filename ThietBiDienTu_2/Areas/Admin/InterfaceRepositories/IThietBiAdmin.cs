using ThietBiDienTu_2.Areas.Admin.ViewModels;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.InterfaceRepositories
{
    public interface IThietBiAdmin
    {
        List<Thietbi> GetTBList();
        Thietbi GetTBById(int _Matb);
        Thietbi CheckSeriExist(string seri, int maDongTb, string oldSeri = "");
        void AddTB(Thietbi cttb);
        void UpdateTB(ThietBiViewAdmin toolDetail);
        void DeleteTB(int MaTB);
        List<Thietbi> GetTbListInThatDay(int matb, DateTime Ngaymuon);
        List<TbFixAndCheck> GetTbFixAndCheckList();
    }
}
