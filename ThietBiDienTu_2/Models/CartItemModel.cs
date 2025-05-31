using System.Drawing;
using System.Net.Http.Headers;

namespace ThietBiDienTu_2.Models
{
    public class CartItemModel
    {
        public int Madongtb { get; set; }
        public string Tendongtb { get; set; }
        public int Soluong { get; set; }
        public string Hinhanh { get; set; }
        public string Trangthai { get; set; }
        public CartItemModel() { }//neu khong bam vao gio hang thi gio hang se trong
        public CartItemModel(Dongthietbi dongthietbi)
        {
            Madongtb = dongthietbi.Madongtb;
            Tendongtb = dongthietbi.Tendongtb;
            Soluong = 1;
            Hinhanh = dongthietbi.Hinhanh;
        }
    }
}
