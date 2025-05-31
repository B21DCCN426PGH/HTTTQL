using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ThietBiDienTu_2.Models;

public partial class Dongthietbi
{
    public int Madongtb { get; set; }

    public string Tendongtb { get; set; } = null!;

    public string Hinhanh { get; set; } = null!;

    public int Soluong { get; set; }

    public string? Mota { get; set; }
    [JsonIgnore]

    public virtual ICollection<Thietbi> Thietbis { get; set; } = new List<Thietbi>();

    [NotMapped]
    public IFormFile hinhanh { get; set; }
}
