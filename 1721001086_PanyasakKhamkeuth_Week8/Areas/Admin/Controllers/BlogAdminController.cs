using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Để sử dụng IFormFile
using System.IO; // Để sử dụng Path.Combine
using Microsoft.AspNetCore.Authorization;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogAdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BlogAdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var blogs = _db.Blogs.ToList(); // Lấy danh sách các bài blog từ cơ sở dữ liệu
            return View(blogs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Blog blog, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine("wwwroot", "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fileStream);
                }

                blog.ImageUrl = "/images/" + uniqueFileName;
            }
            blog.CreatedDate = DateTime.Now;
            _db.Blogs.Add(blog); // Thêm blog vào DbSet
            _db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            var blog = _db.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound(); // Xử lý trường hợp không tìm thấy blog
            }
            return View(blog);
        }

        [HttpPost]
        public IActionResult Edit(Blog blog, IFormFile NewImageFile)
        {
            var existingBlog = _db.Blogs.FirstOrDefault(b => b.Id == blog.Id);
            if (existingBlog == null)
            {
                return NotFound(); // Xử lý trường hợp không tìm thấy blog
            }

            if (NewImageFile != null && NewImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine("wwwroot", "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + NewImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    NewImageFile.CopyTo(fileStream);
                }

                existingBlog.ImageUrl = "/images/" + uniqueFileName;
            }

            existingBlog.Title = blog.Title; // Cập nhật thông tin khác của blog
            existingBlog.Content = blog.Content;
            existingBlog.Author = blog.Author;
            existingBlog.CreatedDate = blog.CreatedDate; // Cập nhật trường CreatedDate
            _db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            return RedirectToAction("Index");
        }




        public IActionResult Detail(int id)
        {
            var blog = _db.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound(); // Xử lý trường hợp không tìm thấy blog
            }
            return View(blog);
        }
        public IActionResult Delete(int id)
        {
            var blog = _db.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound(); // Xử lý trường hợp không tìm thấy blog
            }
            _db.Blogs.Remove(blog); // Xóa blog khỏi DbSet
            _db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            return RedirectToAction("Index");
        }


    }
}
