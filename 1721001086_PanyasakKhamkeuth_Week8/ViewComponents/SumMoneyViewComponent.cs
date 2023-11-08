using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace _1721001086_PanyasakKhamkeuth_Week8.ViewComponents
{
    public class SumMoneyViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public SumMoneyViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            // Kiểm tra xem claim có null hay không
            if (claim != null)
            {
                GioHangViewModel giohang = new GioHangViewModel()
                {
                    DsGioHang = _db.GioHang.Include("SanPham").Where(gh => gh.ApplicationUserId == claim.Value).ToList()
                };

                foreach (var item in giohang.DsGioHang)
                {
                    item.ProductPrice = item.Quantity * item.SanPham.Price;
                    // Cộng thêm tổng thuế và phí ship cho TotalPrice
                    giohang.TotalPrice += item.ProductPrice;
                }

                //giohang.ShippingFee = 30000;
                //giohang.TaxAmount = 0.1;
                //giohang.TaxAmount = giohang.TotalPrice * giohang.TaxAmount;
                giohang.TotalPrice += giohang.ShippingFee + giohang.TaxAmount;


                return View(giohang);
            }

            // Xử lý khi người dùng chưa đăng nhập
            // Ví dụ: Hiển thị giỏ hàng rỗng hoặc chuyển hướng đến trang đăng nhập
            GioHangViewModel emptyCart = new GioHangViewModel();
            return View(emptyCart);
        }
    }
}
