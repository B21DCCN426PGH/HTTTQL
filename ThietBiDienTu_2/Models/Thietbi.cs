using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThietBiDienTu_2.Models;

public partial class Thietbi
{
    public int Matb { get; set; }

    [Required(ErrorMessage ="Vui lòng nhập seri thiết bị")]
    public string Seri { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn phòng")]
    public string Map { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn dòng thiết bị")]
    public int Madongtb { get; set; }

    public string? Trangthai { get; set; }

    public virtual ICollection<Chitietphieumuon> Chitietphieumuons { get; set; } = new List<Chitietphieumuon>();

    [JsonIgnore]
    public virtual Dongthietbi MadongtbNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Phong MapNavigation { get; set; } = null!;

    public virtual ICollection<Chitietphieusua> Chitietphieusuas { get; set; } = new List<Chitietphieusua>();
}
