using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace _1721001086_PanyasakKhamkeuth_Week8.ViewComponents
{
    public class SmallCartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public SmallCartViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public IViewComponentResult Invoke()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var gioHangItems = _db.GioHang.Include("SanPham")
                    .Where(gh => gh.ApplicationUserId == claim.Value)
                    .ToList();

                double totalProductPrice = 0.0; // Initialize total product price

                foreach (var item in gioHangItems)
                {
                    item.ProductPrice = item.Quantity * item.SanPham.Price;
                    totalProductPrice += item.ProductPrice; // Add each product's price to the total product price
                }

                var giohang = new GioHangViewModel
                {
                    DsGioHang = gioHangItems,
                    TotalPrice = totalProductPrice
                };

                return View(giohang);
            }

            // Handle the case when there are no items in the cart
            var emptyCartViewModel = new GioHangViewModel
            {
                DsGioHang = new List<GioHang>()  // Empty list of items
            };

            return View(emptyCartViewModel);
        }


    }
}
