using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Mvc;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Customer.Controllers
{
    [Area("User")]
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BlogController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int page=1)
        {
            
            int pageSize = 6; // Số lượng blog trên mỗi trang
            int totalBlogs = _db.Blogs.Count();

            int totalPages = (int)System.Math.Ceiling((decimal)totalBlogs / pageSize);
            page = page < 1 ? 1 : (page > totalPages ? totalPages : page);

            var blogs = _db.Blogs
                .OrderByDescending(b => b.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(blogs);// Lấy danh sách các blog từ cơ sở dữ liệu
        }
        public IActionResult BlogDetail(int blogId)
        {
            // Lấy dữ liệu của blog có Id tương ứng và truyền nó vào view
            var blog = _db.Blogs.FirstOrDefault(b => b.Id == blogId);
            if (blog == null)
            {
                return NotFound(); // Hoặc xử lý trường hợp không tìm thấy blog
            }
            return View(blog);
        }
    }
}
