using Microsoft.AspNetCore.Mvc;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Customer.Controllers
{
    [Area("User")]
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult chat()
        {
            return View();
        }
    }
}
