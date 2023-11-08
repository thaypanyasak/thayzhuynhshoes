using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _1721001086_PanyasakKhamkeuth_Week8.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TheLoaiController : Controller
	{
		public readonly ApplicationDbContext _db;
		private readonly IWebHostEnvironment _appEnvironment;
		public TheLoaiController(ApplicationDbContext db, IWebHostEnvironment appEnvironment)
		{
			_db = db;
			_appEnvironment = appEnvironment;
		}
		public IActionResult Index()
		{
			{
				var theloai = _db.TheLoai.ToList();
				//var theloai = _db.TheLoai.Where(t => t.Id > 3).ToList();  LINQ lấy ra danh sách có id > 3
				ViewBag.TheLoai = theloai;
			};
			return View();
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(TheLoai theloai, IFormFile ImageUrl)
		{

			////Tên tệp tin gốc của tệp ảnh vừa tải lên
			//var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageUrl.FileName);

			////_appEnvironment.WebRootPath cho biết thư mục gốc của ứng dụng web, và "images" là thư mục để lưu ảnh
			//var imagePath = Path.Combine(_appEnvironment.WebRootPath, "images", uniqueFileName);

			////FileStream  sao chép nội dung của tệp ảnh từ ImageUrl vào lưu trữ tệp ảnh trên máy chủ.
			//using (var stream = new FileStream(imagePath, FileMode.Create))
			//{
			//	ImageUrl.CopyTo(stream);
			//}

			//theloai.ImageUrl = uniqueFileName;
			// Thêm thông tin vào bảng TheLoai
			_db.TheLoai.Add(theloai);
			// Lưu lại
			_db.SaveChanges();


			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			if (id == 0)
			{
				return NotFound();
			}
			var theloai = _db.TheLoai.Find(id);
			return View(theloai);
		}
		[HttpPost]
		public IActionResult Edit(TheLoai theloai, IFormFile ImageUrl)
		{
			////Tên tệp tin gốc của tệp ảnh vừa tải lên
			//var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageUrl.FileName);

			////_appEnvironment.WebRootPath cho biết thư mục gốc của ứng dụng web, và "images" là thư mục để lưu ảnh
			//var imagePath = Path.Combine(_appEnvironment.WebRootPath, "images", uniqueFileName);

			////FileStream  sao chép nội dung của tệp ảnh từ ImageUrl vào lưu trữ tệp ảnh trên máy chủ.
			//using (var stream = new FileStream(imagePath, FileMode.Create))
			//{
			//	ImageUrl.CopyTo(stream);
			//}

			//theloai.ImageUrl = uniqueFileName;
			// Thêm thông tin vào bảng TheLoai
			_db.TheLoai.Update(theloai);
			// Lưu lại
			_db.SaveChanges();


			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult Delete(int id)
		{
			if (id == 0)
			{
				return NotFound();
			}
			var theloai = _db.TheLoai.Find(id);
            return RedirectToAction("Index");
        }
		[HttpPost]
		public IActionResult DeleteConfirm(int id)
		{

			var theloai = _db.TheLoai.Find(id);
			if (theloai == null)
			{
				return NotFound();
			}

			// Thêm thông tin vào bảng TheLoai
			_db.TheLoai.Remove(theloai);
			// Lưu lại
			_db.SaveChanges();
            return Json(new { success = true });
        }

		[HttpGet]
		public IActionResult Detail(int id)
		{
			if (id == 0)
			{
				return NotFound();
			}
			var theloai = _db.TheLoai.Find(id);
			if (theloai == null)
			{
				return NotFound();
			}
			return View(theloai);
		}
    }
}
