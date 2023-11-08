using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Linq;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SanPhamController : Controller
	{
		private readonly ApplicationDbContext _db;


        public SanPhamController(ApplicationDbContext db)
		{
			_db = db;
        }

        public IActionResult Index()
	{
            var productsWithNhaCungCap = _db.SanPham.Include(sp => sp.NhaCungCap).ToList();
            return View(productsWithNhaCungCap);
        }

		[HttpGet]
		public IActionResult Upsert(int id)
		{
			ViewBag.id = id;
			ViewBag.theloai = _db.TheLoai.ToList();
            ViewBag.nhaCungCaps = _db.NhaCungCap.ToList();
            ViewBag.sanPhamTheLoais = _db.sanphamTheloais.ToList();

            if (id == 0)
			{

				return View();
			}
			else
			{
				var a = _db.SanPham.Find(id);
				return View(a);
			}
		}

        [HttpPost]
        public IActionResult Upsert(SanPham data, IFormFile mainImage, List<IFormFile> secondaryImages, List<int> theloaii, int nhaCungCapId, IFormFile videoFile)
        {
            ViewBag.nhaCungCaps = _db.NhaCungCap.ToList();
            ViewBag.theloai = _db.TheLoai.ToList();

            if (data.Id == 0)
            {
                // Process and save the main image if provided
                if (mainImage != null && mainImage.Length > 0)
                {
                    string mainImageFileName = GenerateUniqueFileName(mainImage.FileName);
                    string mainImageFolderPath = Path.Combine("wwwroot/images/", mainImageFileName);
                    data.ImageUrl = Url.Content("~/images/" + mainImageFileName);

                    using (var stream = new FileStream(mainImageFolderPath, FileMode.Create))
                    {
                        mainImage.CopyTo(stream);
                    }
                }

                // Process and save the video if provided
                if (videoFile != null && videoFile.Length > 0)
                {
                    string videoFileName = GenerateUniqueFileName(videoFile.FileName);
                    string videoFolderPath = Path.Combine("wwwroot/videos/", videoFileName);
                    data.VideoUrl = Url.Content("~/videos/" + videoFileName);

                    using (var stream = new FileStream(videoFolderPath, FileMode.Create))
                    {
                        videoFile.CopyTo(stream);
                    }
                }
                else
                {
                    // Set a default video URL for new products when no video is provided
                    data.VideoUrl = Url.Content("~/videos/Cosmic Unity 3 Basketball Shoes. Nike VN_4e57dfd8-0719-4087-8264-1ee7ea62f5c7.mp4");
                }

                _db.SanPham.Add(data);
                _db.SaveChanges();

                foreach (var item in theloaii)
                {
                    var newSanphamTheloai = new SanphamTheLoai
                    {
                        SanphamId = data.Id,
                        TheloaiId = item
                    };
                    _db.sanphamTheloais.Add(newSanphamTheloai);
                }
                data.NhaCungCapId = nhaCungCapId;
            }
            else
            {
                // If a new main image is uploaded, update the main image URL
                if (mainImage != null && mainImage.Length > 0)
                {
                    string mainImageFileName = GenerateUniqueFileName(mainImage.FileName);
                    string mainImageFolderPath = Path.Combine("wwwroot/images/", mainImageFileName);
                    data.ImageUrl = Url.Content("~/images/" + mainImageFileName);

                    using (var stream = new FileStream(mainImageFolderPath, FileMode.Create))
                    {
                        mainImage.CopyTo(stream);
                    }
                }

                // Process and update the video if provided
                if (videoFile != null && videoFile.Length > 0)
                {
                    string videoFileName = GenerateUniqueFileName(videoFile.FileName);
                    string videoFolderPath = Path.Combine("wwwroot/videos/", videoFileName);
                    data.VideoUrl = Url.Content("~/videos/" + videoFileName);

                    using (var stream = new FileStream(videoFolderPath, FileMode.Create))
                    {
                        videoFile.CopyTo(stream);
                    }
                }

                _db.SanPham.Update(data);
            }

            // Handle secondary images
            if (secondaryImages != null && secondaryImages.Count > 0)
            {
                foreach (var secondaryImage in secondaryImages)
                {
                    string secondaryImageFileName = GenerateUniqueFileName(secondaryImage.FileName);
                    string secondaryImageFolderPath = Path.Combine("wwwroot/images/", secondaryImageFileName);

                    // Create a record for the secondary image and link it to the product
                    var secondaryProductImage = new ProductImage
                    {
                        SanPhamId = data.Id,
                        ImageUrl = Url.Content("~/images/" + secondaryImageFileName),
                        IsMainImage = false // Mark it as a secondary image
                    };
                    data.NhaCungCapId = nhaCungCapId;
                    using (var stream = new FileStream(secondaryImageFolderPath, FileMode.Create))
                    {
                        secondaryImage.CopyTo(stream);
                    }

                    _db.productImages.Add(secondaryProductImage);
                }
            }

            var secondaryImagesForProduct = _db.productImages.Where(pi => pi.SanPhamId == data.Id).ToList();

            _db.SaveChanges();

            return RedirectToAction("index");
        }



        // Hàm tạo tên file duy nhất
        private string GenerateUniqueFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string baseFileName = Path.GetFileNameWithoutExtension(fileName);
            string uniqueFileName = baseFileName + "_" + Guid.NewGuid().ToString() + extension;
            return uniqueFileName;
        }


        [HttpPost]
		public IActionResult Delete(int id)
		{
            var product = _db.SanPham.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // Xóa sản phẩm
            _db.SanPham.Remove(product);
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
            var sanpham = _db.SanPham.Include(sp => sp.Images).SingleOrDefault(sp => sp.Id == id);
            if (sanpham == null)
			{
				return NotFound();
			}
            return View(sanpham);
		}

		public IActionResult Datatables()
		{
            var data = _db.SanPham.Include("TheLoai").ToList();
            return Json(new { data });
        }
		[HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _db.SanPham.Include(sp => sp.Images).FirstOrDefault(sp => sp.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.sanPhamTheLoais = _db.sanphamTheloais.ToList();
            ViewBag.theloai = _db.TheLoai.ToList();
            ViewBag.nhaCungCaps = _db.NhaCungCap.ToList();
            ViewBag.ExistingVideoUrl = product.VideoUrl; // Pass the existing video URL to the view

            return View(product);
        }


        [HttpPost]
        public IActionResult Edit(SanPham data, IFormFile mainImage, List<IFormFile> secondaryImages, List<int> theloaii, int nhaCungCapId, double rating, IFormFile videoFile)
        {
            ViewBag.nhaCungCaps = _db.NhaCungCap.ToList();
            ViewBag.theloai = _db.TheLoai.ToList();

            // Create a model with a default rating value
            var model = new SanPham();
            model.Rating = 0;

            try
            {
                // Get the existing product from the database, including its images
                var existingProduct = _db.SanPham.Include(p => p.Images).FirstOrDefault(p => p.Id == data.Id);

                // Process and update the main image if provided or retain the existing one
                if (mainImage != null && mainImage.Length > 0)
                {
                    string mainImageFileName = GenerateUniqueFileName(mainImage.FileName);
                    string mainImageFolderPath = Path.Combine("wwwroot/images/", mainImageFileName);
                    data.ImageUrl = Url.Content("~/images/" + mainImageFileName);

                    using (var stream = new FileStream(mainImageFolderPath, FileMode.Create))
                    {
                        mainImage.CopyTo(stream);
                    }
                }
                else
                {
                    // Retain the existing main image URL
                    data.ImageUrl = existingProduct.ImageUrl;
                }

                // Process and update the video if provided
                if (videoFile != null && videoFile.Length > 0)
                {
                    // Delete the existing video file
                    if (!string.IsNullOrEmpty(existingProduct.VideoUrl))
                    {
                        string existingVideoFilePath = Path.Combine("wwwroot", existingProduct.VideoUrl.TrimStart('~'));
                        if (System.IO.File.Exists(existingVideoFilePath))
                        {
                            System.IO.File.Delete(existingVideoFilePath);
                        }
                    }

                    string videoFileName = GenerateUniqueFileName(videoFile.FileName);
                    string videoFolderPath = Path.Combine("wwwroot/videos/", videoFileName);
                    data.VideoUrl = Url.Content("~/videos/" + videoFileName);

                    using (var stream = new FileStream(videoFolderPath, FileMode.Create))
                    {
                        videoFile.CopyTo(stream);
                    }
                }
                else
                {
                    // Retain the existing video URL if no new video is uploaded
                    data.VideoUrl = existingProduct.VideoUrl;
                }

                // Clear the existing secondary images if new secondary images are provided
                if (secondaryImages != null && secondaryImages.Count > 0)
                {
                    existingProduct.Images.Clear();

                    // Process and update secondary images
                    foreach (var secondaryImage in secondaryImages)
                    {
                        if (secondaryImage != null && secondaryImage.Length > 0)
                        {
                            string secondaryImageFileName = GenerateUniqueFileName(secondaryImage.FileName);
                            string secondaryImageFolderPath = Path.Combine("wwwroot/images/", secondaryImageFileName);
                            var secondaryImageUrl = Url.Content("~/images/" + secondaryImageFileName);

                            using (var stream = new FileStream(secondaryImageFolderPath, FileMode.Create))
                            {
                                secondaryImage.CopyTo(stream);
                            }

                            // Create a new ProductImage for each secondary image
                            var productImage = new ProductImage
                            {
                                ImageUrl = secondaryImageUrl,
                                SanPhamId = data.Id,
                                IsMainImage = false
                            };

                            _db.productImages.Add(productImage);
                        }
                    }
                }
                else
                {
                    // Retain the existing secondary images
                    data.Images = existingProduct.Images;
                }

                // Update the rating
                data.Rating = rating;

                // Update other properties of the SanPham entity as needed

                // Update the SanPham entity
                _db.Entry(existingProduct).CurrentValues.SetValues(data);

                // Save changes to the database
                _db.SaveChanges();

                // Redirect to the detail view or another appropriate action
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle errors and add an error message to the model state
                ModelState.AddModelError("", "An error occurred while saving the data: " + ex.Message);

                // If an error occurred, return to the edit view with the provided data
                return View(data);
            }
        }


        [HttpPost]
        public IActionResult RateProduct(int id, double rating)
        {
            var product = _db.SanPham.Find(id);
            if (product == null)
                return NotFound();

            // Update the product's rating
            product.Rating = rating;

            // Save changes to the database
            _db.SaveChanges();

            // Redirect back to the product detail page
            return RedirectToAction("Detail", new { id });
        }







    }
}
