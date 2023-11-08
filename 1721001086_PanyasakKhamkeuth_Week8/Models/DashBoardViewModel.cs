namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class DashBoardViewModel
    {
        public string SelectedMenuItem { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<SanPham> SanPhams { get; set; }
        public List<TheLoai> TheLoais { get; set; }
        public List<NhaCungCap> NhaCungCaps { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<HoaDon> DonHangs { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalMessages { get; set; }
        public int TotalAccounts { get; set; }

        // Add properties for other data types you want to manage
    }
}
