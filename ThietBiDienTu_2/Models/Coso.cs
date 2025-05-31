using System;
using System.Collections.Generic;

namespace ThietBiDienTu_2.Models;

public partial class Coso
{
    public int Macs { get; set; }

    public string Tencs { get; set; } = null!;

    public string Diachi { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
