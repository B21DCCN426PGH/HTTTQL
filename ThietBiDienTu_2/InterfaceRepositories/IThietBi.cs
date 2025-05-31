using ThietBiDienTu_2.Models.ViewModels;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Repositories;

namespace ThietBiDienTu_2.InterfaceRepositories
{
    public interface IThietBi
    {
        List<Thietbi> GetTBList();
        Thietbi GetTBById(int _Matb);
        Thietbi CheckSeriExist(string seri, int maDongTb, string oldSeri = "");
        void AddTB(Thietbi cttb);
        void UpdateTB(ThietBiView toolDetail);
        void DeleteTB(int MaTB);
        List<Thietbi> GetTbListInThatDay(int matb, DateTime Ngaymuon);
        List<TbFixAndCheck> GetTbFixAndCheckList();
    }
}
