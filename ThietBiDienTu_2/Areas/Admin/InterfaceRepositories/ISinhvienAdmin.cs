using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.InterfaceRepositories
{
    public interface ISinhvienAdmin
    {
        List<Sinhvien> GetSinhvienList();
        void Add(Sinhvien sinhvien);
        void Update(Sinhvien sinhvien);
        void Delete(Sinhvien sinhvien);
        Sinhvien FindSinhvien(int id);
        Sinhvien GetSvById(int v);
    }
}
