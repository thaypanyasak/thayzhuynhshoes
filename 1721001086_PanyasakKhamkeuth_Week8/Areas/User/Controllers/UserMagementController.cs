using _1721001086_PanyasakKhamkeuth_Week8.Data;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.User.Controllers
{
    [Area("User")]
    public class UserMagementController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserMagementController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)

            {
                if (User.Identity.IsAuthenticated)
                {
                    var inden = (ClaimsIdentity)User.Identity;
                    var datauser = inden.FindFirst(ClaimTypes.NameIdentifier);
                    var tk = datauser.Value;

                    ViewBag.data = _db.ApplicationUser.FirstOrDefault(x => x.Id == tk);
                    ViewBag.hoadon = _db.HoaDon.Where(x => x.ApplicationUserId == tk).ToList();

                    return View();
                }


            }
            return Redirect("/Identity/Account/Login");
        }
        public IActionResult ChangeProfile(string iduser, IFormFile file)
        {

            var a = _db.ApplicationUser.FirstOrDefault(x => x.Id == iduser);
            if (file != null && file.Length > 0)
            {
                string folderPath = Path.Combine("wwwroot/profile/", file.FileName);

                a.ImageUrl = Url.Content("~/profile/" + file.FileName);
                //updatedanhgia.img = Url.Content("~/uploads/" + file.FileName);


                using (var stream = new FileStream(folderPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            _db.ApplicationUser.Update(a);
            //_db.danhgia.Update(updatedanhgia);

            _db.SaveChanges();
            return Json(new { success = true });

        }
        public IActionResult ChangeUser(string Name, string Email, string phone, string Address)
        {
            var inden = (ClaimsIdentity)User.Identity;
            var datauser = inden.FindFirst(ClaimTypes.NameIdentifier);
            var tk = datauser.Value;

            var data = _db.ApplicationUser.FirstOrDefault(x => x.Id == tk);
            if (Name != null)
            {
                data.Name = Name;
                _db.Update(data);
            }
            if (Email != null)
            {
                data.Email = Email;
                _db.Update(data);
            }
            if (phone != null)
            {
                data.PhoneNumber = phone;
                _db.Update(data);
            }
            if (Address != null)
            {
                data.Address = Address;
                _db.Update(data);

            }
            _db.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult GetChiTiet(int id)
        {
            var chiTietHoaDon = _db.ChiTietHoaDon.Include("Sanpham").Where(x => x.HoaDonId == id).ToList();
            ViewBag.hoadon = chiTietHoaDon;
            return Json(chiTietHoaDon);
        }
    }
}
