using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.InterfaceRepositories
{
    public interface IPhongAdmin
    {
        List<Phong> GetPhongList();
        List<Phong> GetPhongListByCoso(string macs);
        void Add(Phong phong);
        void Update(Phong phong, string oldMap);
        void Delete(string maphong);
        Phong FindPhong(string Map);
        List<Phong> phongKhoListOfTbList(List<Thietbi> tbList);
        bool CheckIfPhongHasDevices(string id);
        bool CheckPhongExist(string newMap, string oldMap = "");
        //object GetDevicesInPhong(string phongId);
    }
}
