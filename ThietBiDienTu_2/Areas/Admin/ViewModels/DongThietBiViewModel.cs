using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThietBiDienTu_2.Areas.Admin.ViewModels
{
    public class DongThietBiViewModel
    {
        public int Madongtb { get; set; }

        public string Tendongtb { get; set; }

        public string? Hinhanh { get; set; }

        public int Soluong { get; set; }

        public int SoLuongHoatDong { get; set; }

        public int SoLuongTonKho { get; set; }

        public int SoLuongHu { get; set; }

        public int SoluongMuon {  get; set; }

        public int SoluongSua {  get; set; }

        public int SoluongTL {  get; set; }

        public bool NeedsMoreDevices { get; set; }

        public string? Mota { get; set; }

        [NotMapped]
        public IFormFile hinhanh { get; set; }

        public DateTime YourDateProperty { get; set; }

    }
}