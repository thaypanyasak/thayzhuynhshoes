using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashBoardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashBoardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            ViewBag.SanPham = _db.SanPham.ToList();
            int totalOrders = CalculateTotalOrders();
            int totalProducts = CalculateTotalProducts();
            int totalMessages = CalculateTotalMessages();
            int totalAccounts = CalculateTotalAccounts();

            var dashboardViewModel = new DashBoardViewModel
            {
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                TotalMessages = totalMessages,
                TotalAccounts = totalAccounts
            };

            return View(dashboardViewModel);
        }
        private int CalculateTotalOrders()
        {
            var donhang = _db.ChiTietHoaDon.Count(); // _dbContext là đối tượng DbContext của Entity Framework

            return donhang;
        }

        private int CalculateTotalProducts()
        {
            var products = _db.SanPham.Count(); // _dbContext là đối tượng DbContext của Entity Framework

            return products;
        }

        private int CalculateTotalMessages()
        {
            var message = _db.Messages.Count(); // _dbContext là đối tượng DbContext của Entity Framework

            return message;
        }

        private int CalculateTotalAccounts()
        {
            var account = _db.Users.Count(); // _dbContext là đối tượng DbContext của Entity Framework

            return account;
        }

    }
}
