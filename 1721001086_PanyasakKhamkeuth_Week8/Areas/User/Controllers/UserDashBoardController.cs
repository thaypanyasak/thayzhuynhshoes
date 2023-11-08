using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNet.SignalR;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.User.Controllers
{
    [Area("User")]
    public class UserDashBoardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserDashBoardController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            // Lấy thông tin người dùng hiện tại
            var user = _userManager.GetUserAsync(User).Result;

            return View(user);
        }
    }
}
