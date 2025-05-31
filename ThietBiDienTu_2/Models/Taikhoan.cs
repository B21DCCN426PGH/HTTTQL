using System;
using System.Collections.Generic;

namespace ThietBiDienTu_2.Models;

public partial class Taikhoan
{
    public int Matk { get; set; }

    public string Matkhau { get; set; } = null!;

    public short Loaitk { get; set; }

    public virtual Nhanvien? Nhanvien { get; set; }

    public virtual Sinhvien? Sinhvien { get; set; }
}
