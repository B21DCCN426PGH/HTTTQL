using System;
using System.Collections.Generic;

namespace ThietBiDienTu_2.Models;

public partial class Phieusua
{
    public int Maps { get; set; }

    public DateTime Ngaylap { get; set; }

    public int Trangthai { get; set; }

    public decimal? Tongchiphi { get; set; }

    public virtual ICollection<Chitietphieusua> Chitietphieusuas { get; set; } = new List<Chitietphieusua>();

}
