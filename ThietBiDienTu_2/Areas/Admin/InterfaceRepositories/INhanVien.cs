using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.InterfaceRepositories
{
	public interface INhanVien
	{
		Nhanvien GetCurrentNhanVien(int manv);
	}
}
