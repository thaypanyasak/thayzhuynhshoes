using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminDashBoardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashBoardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index(string selectedMenuItem)
        {
            var model = new DashBoardViewModel
            {
                SelectedMenuItem = selectedMenuItem,
                Users = _db.Users.ToList(),
                SanPhams = _db.SanPham.ToList(),
                TheLoais = _db.TheLoai.ToList(),
                NhaCungCaps = _db.NhaCungCap.ToList(),
                Blogs = _db.Blogs.ToList(),
                DonHangs = _db.HoaDon.ToList(),


                // Load data for other items as needed
            };

            return View(model);
        }
    }
}
