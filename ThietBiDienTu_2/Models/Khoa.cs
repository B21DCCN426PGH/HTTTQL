using System;
using System.Collections.Generic;

namespace ThietBiDienTu_2.Models;

public partial class Khoa
{
    public int Makhoa { get; set; }

    public string Tenkhoa { get; set; } = null!;

    public virtual ICollection<Sinhvien> Sinhviens { get; set; } = new List<Sinhvien>();
}
