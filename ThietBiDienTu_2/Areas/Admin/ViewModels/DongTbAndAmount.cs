using System.Text.Json.Serialization;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Areas.Admin.ViewModels
{
    public class DongTbAndAmount
    {
        public int madongtb {  get; set; }
        public string tendongtb {  get; set; }
        public string hinhanh {  get; set; }
        public int amount {  get; set; }
    }
}
