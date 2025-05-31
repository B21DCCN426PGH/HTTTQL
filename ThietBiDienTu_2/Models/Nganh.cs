using System;
using System.Collections.Generic;

namespace ThietBiDienTu_2.Models;

public partial class Nganh
{
    public int Manganh { get; set; }

    public string Tennganh { get; set; } = null!;

    public virtual ICollection<Sinhvien> Sinhviens { get; set; } = new List<Sinhvien>();
}
