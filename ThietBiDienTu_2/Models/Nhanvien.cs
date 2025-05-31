using System;
using System.Collections.Generic;

namespace ThietBiDienTu_2.Models;

public partial class Nhanvien
{
    public int Manv { get; set; }

    public string Tennv { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string Gioitinh { get; set; } = null!;

    public DateTime Ngaysinh { get; set; }

    public string Email { get; set; } = null!;

    public string? Diachi { get; set; }

    public virtual Taikhoan ManvNavigation { get; set; } = null!;

    public virtual ICollection<Phieumuon> Phieumuons { get; set; } = new List<Phieumuon>();
}
