using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.ViewModels;

namespace ThietBiDienTu_2.InterfaceRepositories
{
    public interface IDongThietBi
    {
        List<Dongthietbi> GetAllDongThietBi();
        void Updatedtb(Dongthietbi dongthietbi);
        bool CountSlFullOrNot(int MaDongTb);
        Dongthietbi GetDtbById(int id);
        List<DongTbAndAmount> DongTbAndAmountTbInDay(DateTime Ngaymuon);
    }
}
