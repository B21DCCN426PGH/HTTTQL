using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThietBiDienTu_2.Models;

public partial class Sinhvien
{
    [Required(ErrorMessage ="Vui lòng nhâp mã sinh viên")]
    public int Masv { get; set; }

    [Required(ErrorMessage = "Vui lòng nhâp tên sinh viên")]
    public string Tensv { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn khoa")]
    public int Makhoa { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngành")]
    public int Manganh { get; set; }

    [Required(ErrorMessage = "Vui lòng nhâp email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhâp sdt")]
    public string Sdt { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhâp ngày sinh")]
    public DateTime Ngaysinh { get; set; }

    public string? Diachi { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giới tính")]
    public string Gioitinh { get; set; } = null!;

    public virtual Khoa MakhoaNavigation { get; set; } = null!;

    public virtual Nganh ManganhNavigation { get; set; } = null!;

    public virtual Taikhoan MasvNavigation { get; set; } = null!;

    public virtual ICollection<Phieumuon> Phieumuons { get; set; } = new List<Phieumuon>();
}
