namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class GioHangViewModel
    {
        public IEnumerable<GioHang> DsGioHang { get; set; }
        public double TotalPrice { get; set; }
        public double ShippingFee { get; set; }
        public double TaxAmount { get; set; }
        public HoaDon HoaDon { get; set; }
        // Add a property to hold product images

    }
}
