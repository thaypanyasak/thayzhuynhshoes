using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Customer.Controllers
{
    [Area("User")]
    public class FavorProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        public FavorProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public IActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            var favorProducts = _db.FavorProducts
                .Include(fp => fp.SanPham)
                .Where(fp => fp.ApplicationUserId == userId)
                .ToList();

            return View(favorProducts);
        }

        // Trong FavorProductController.cs
        public IActionResult AddToFavorites(int sanphamId)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            // Kiểm tra xem sản phẩm đã tồn tại trong danh sách yêu thích của người dùng chưa
            var existingFavorProduct = _db.FavorProducts.FirstOrDefault(fp => fp.SanPhamId == sanphamId && fp.ApplicationUserId == userId);

            if (existingFavorProduct == null)
            {
                // Nếu sản phẩm chưa tồn tại trong danh sách yêu thích, hãy thêm nó
                var favorProduct = new FavorProduct
                {
                    SanPhamId = sanphamId,
                    ApplicationUserId = userId
                };

                _db.FavorProducts.Add(favorProduct);
                _db.SaveChanges();
            }

            // Redirect hoặc trả về JSON response để xử lý UI theo ý của bạn
            // Ví dụ: Trả về một JSON response để cập nhật giao diện người dùng
            return Json(new { success = true });
        }


        // Xóa sản phẩm khỏi danh sách yêu thích
        public IActionResult Delete(int sanphamId)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            // Tìm FavorProduct tương ứng để xóa
            var favorProduct = _db.FavorProducts.FirstOrDefault(fp => fp.SanPhamId == sanphamId && fp.ApplicationUserId == userId);

            if (favorProduct != null)
            {
                // Xóa FavorProduct khỏi cơ sở dữ liệu
                _db.FavorProducts.Remove(favorProduct);
                _db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Sản phẩm không tồn tại trong danh sách yêu thích của bạn." });


        }
    }
}
