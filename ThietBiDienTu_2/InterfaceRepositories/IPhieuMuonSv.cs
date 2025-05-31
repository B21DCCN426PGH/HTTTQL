
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.ViewModels;
namespace ThietBiDienTu_2.InterfaceRepositories
{
    public interface IPhieuMuonSv
    {
        List<Phieumuon> GetAllPhieuMuonBySv();
        List<Phieumuon> GetPhieuMuonList();
        void Add(Phieumuon phieumuon);
        void Update(Phieumuon phieumuon);
        void Delete(Phieumuon phieumuon);
        Phieumuon FindPhieuMuon(string Mapm);
        List<int> AllStatePhieuMuonToday();
    }
}
