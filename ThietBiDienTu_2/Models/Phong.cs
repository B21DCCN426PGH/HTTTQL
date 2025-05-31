using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThietBiDienTu_2.Models;

public partial class Phong
{
    [Required(ErrorMessage = "Vui lòng nhập mã phòng")]
    public string Map { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn cơ sở")]
    public int Macs { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên phòng")]
    public string Tenphong { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn loại phòng")]
    public string Loaiphong { get; set; } = null!;

    public int? Douutien { get; set; }

    public virtual Coso MacsNavigation { get; set; } = null!;

    public virtual ICollection<Thietbi> Thietbis { get; set; } = new List<Thietbi>();
}
