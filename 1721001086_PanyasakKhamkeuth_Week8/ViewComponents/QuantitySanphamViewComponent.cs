using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Mvc;

namespace _1721001086_PanyasakKhamkeuth_Week8.ViewComponents
{
    public class QuantitySanphamViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public QuantitySanphamViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke(GioHang gioHangItem)
        {
            return View(gioHangItem);
        }
    }
}
