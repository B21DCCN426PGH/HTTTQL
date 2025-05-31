using System.Drawing;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.Repositories
{
	public class NhanvienAdminRepo : INhanVien
	{
		ToolDbContext context;
		public NhanvienAdminRepo(ToolDbContext context)
		{
			this.context = context;
		}

		public Nhanvien GetCurrentNhanVien(int manv)
		{
			Nhanvien nv = context.Nhanviens.Find(manv);
			return nv;
		}

	}
}
