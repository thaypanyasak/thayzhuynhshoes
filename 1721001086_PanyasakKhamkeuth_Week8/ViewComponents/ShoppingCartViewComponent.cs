using _1721001086_PanyasakKhamkeuth_Week8.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using _1721001086_PanyasakKhamkeuth_Week8.Models;

namespace _1721001086_PanyasakKhamkeuth_Week8.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke(GioHang gioHangItem)
        {
            return View(gioHangItem);
        }
    }
}
