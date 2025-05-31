using System;
using System.Collections.Generic;

namespace ThietBiDienTu_2.Models;

public partial class Chitietphieumuon
{
    public int Matb { get; set; }

    public int Mapm { get; set; }

    public DateTime? Ngaytra { get; set; }

    public virtual Phieumuon MapmNavigation { get; set; } = null!;

    public virtual Thietbi MatbNavigation { get; set; } = null!;
}
