using Microsoft.EntityFrameworkCore;
using ThietBiDienTu_2.InterfaceRepositories;
using ThietBiDienTu_2.Models;
using ThietBiDienTu_2.Models.ViewModels;

namespace ThietBiDienTu_2.Repositories
{
    public class PhieuMuonRepo : IPhieuMuonSv
    {
        ToolDbContext _toolDbContext;
        public PhieuMuonRepo(ToolDbContext toolDbContext)
        {
            _toolDbContext = toolDbContext;

        }



        public PhieuMuonViewModel GetPhieumuonViewById(int mapm)
        {
            throw new NotImplementedException();
        }

        public void Add(Phieumuon phieumuon)
        {
            throw new NotImplementedException();
        }

        public void Update(Phieumuon phieumuon)
        {
            throw new NotImplementedException();
        }

        public void Delete(Phieumuon phieumuon)
        {
            throw new NotImplementedException();
        }

        public Phieumuon FindPhieuMuon(string Mapm)
        {
            throw new NotImplementedException();
        }

        public List<Phieumuon> GetAllPhieuMuonBySv()
        {
            // Giả sử bạn có một thuộc tính `MaSv` trong bảng Phieumuon để nhận diện sinh viên
            return _toolDbContext.Phieumuons.ToList();
        }

        public List<Phieumuon> GetPhieuMuonList()
        {
            throw new NotImplementedException();
        }
        public List<int> AllStatePhieuMuonToday()
        {
            DateTime today = DateTime.Now.Date;
            List<int> trangthaiToday = _toolDbContext.Phieumuons.Where(x => x.Ngaymuon == today).Select(x => x.Trangthai).Distinct().ToList();

            return trangthaiToday;


        }
    }
}

