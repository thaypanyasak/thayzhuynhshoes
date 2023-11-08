using System.Diagnostics;
using System.Security.Claims;
using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace _1721001086_PanyasakKhamkeuth_Week8.Controllers
{
    [Area("User")]
    public class ShopController : Controller
	{
		private readonly ILogger<ShopController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _appEnvironment;
        public class CategoryWithCount
        {
            public TheLoai Category { get; set; }
            public int ProductCount { get; set; }
        }
        public class BrandWithCount
        {
            public NhaCungCap Brand { get; set; }
            public int ProductCount { get; set; }
        }
        private IEnumerable<BrandWithCount> GetBrandsWithCount()
        {
            var brands = _db.NhaCungCap
                .Select(brand => new BrandWithCount
                {
                    Brand = brand,
                    ProductCount = _db.SanPham.Count(sp => sp.NhaCungCapId == brand.Id)
                })
                .ToList();

            return brands;
        }
        private IEnumerable<CategoryWithCount> GetCategoriesWithCount()
        {
            var categories = _db.TheLoai
                .Select(category => new CategoryWithCount
                {
                    Category = category,
                    ProductCount = _db.sanphamTheloais.Count(st => st.TheloaiId == category.Id)
                })
                .ToList();

            return categories;
        }

        public ShopController(ILogger<ShopController> logger,ApplicationDbContext db, IWebHostEnvironment appEnvironment)
		{
			_db = db;
			_logger = logger;
			_appEnvironment = appEnvironment;
		}
        private IEnumerable<NhaCungCap> GetBrands()
        {
            return _db.NhaCungCap.ToList();
        }

        private IEnumerable<TheLoai> GetCategories()
        {
            return _db.TheLoai.ToList();
        }

        public IActionResult Index(int? category, int? brand, decimal? minPrice, decimal? maxPrice, string tag, string brandName, string categoryName, string searchText, int page = 1)
        {
            // Lấy danh sách thể loại và nhà cung cấp
            ViewBag.Categories = GetCategoriesWithCount();
            ViewBag.Brands = GetBrandsWithCount();
            int totalAvailableProducts = _db.SanPham.Count();
            IQueryable<SanPham> sanPhamQuery = _db.SanPham;
            
            // Lọc theo thể loại
            if (category.HasValue)
            {
                var sanphamTheLoais = _db.sanphamTheloais
                    .Where(st => st.TheloaiId == category.Value)
                    .Select(st => st.SanphamId)
                    .ToList();
                sanPhamQuery = sanPhamQuery.Where(sp => sanphamTheLoais.Contains(sp.Id));
            }

            // Lọc theo nhà cung cấp
            if (brand.HasValue)
            {
                sanPhamQuery = sanPhamQuery.Where(sp => sp.NhaCungCapId == brand.Value);
            }

            // Filter by tag (product name)
            if (!string.IsNullOrEmpty(tag))
            {
                sanPhamQuery = sanPhamQuery.Where(sp => sp.Name.Contains(tag));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                sanPhamQuery = sanPhamQuery
                    .Where(sp => sp.Name.Contains(searchText) ||
                                 sp.NhaCungCap.Name.Contains(searchText) ||
                                 sp.Theloais.Any(tl => tl.Theloai.Name.Contains(searchText)));
            }
            // Lọc theo giá
            if (minPrice.HasValue)
            {
                sanPhamQuery = sanPhamQuery.Where(sp => sp.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                sanPhamQuery = sanPhamQuery.Where(sp => sp.Price <= maxPrice.Value);
            }

            // Tính toán tổng số sản phẩm và số sản phẩm trên mỗi trang
            int totalProducts = sanPhamQuery.Count();
            int pageSize = 12;
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Pagination
            int skip = (page - 1) * pageSize;

            var products = sanPhamQuery
                .OrderBy(sp => sp.Id) // Bạn có thể sắp xếp theo thuộc tính phù hợp
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            ViewBag.SanPham = products;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalAvailableProducts = totalAvailableProducts;

            return View();
        }

        


        public IActionResult Privacy()
		{
			return View();
		}
        public IActionResult Details(int sanphamId)
        {
            // Lấy danh sách sản phẩm ngẫu nhiên (4 sản phẩm) từ cơ sở dữ liệu
            var randomSanPhams = _db.SanPham.OrderBy(x => Guid.NewGuid()).Take(4).ToList();

            // Lựa chọn sản phẩm cụ thể dựa trên sanphamId
            var sanpham = _db.SanPham.Find(sanphamId);

            var danhmuc = _db.sanphamTheloais.Include("Theloai").Where(x => x.SanphamId == sanphamId);

            // Lấy danh sách nhà cung cấp cho sản phẩm này
            var nhaCungCaps = _db.SanPham
                .Where(sp => sp.Id == sanphamId)
                .Select(sp => sp.NhaCungCap)
                .ToList();

            var secondaryImagesForProduct = _db.productImages.Where(pi => pi.SanPhamId == sanphamId).ToList();

            GioHang giohang = new GioHang()
            {
                SanPhamId = sanphamId,
                SanPham = sanpham,
                Quantity = 1,
                SecondaryImages = secondaryImagesForProduct,
                NhaCungCaps = nhaCungCaps // Gán danh sách nhà cung cấp cho sản phẩm
            };

            ViewBag.danhmuc = danhmuc;
            ViewBag.RandomSanPhams = randomSanPhams; // Truyền danh sách sản phẩm ngẫu nhiên đến view

            return View(giohang);
        }



        [HttpPost]
        [Authorize]
        public IActionResult Details(int sanphamId, GioHang giohang, string size)
        {
            Console.WriteLine("Received size: " + size);
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            giohang.ApplicationUserId = claim.Value;

            giohang.Size = size;

            var giohangdb = _db.GioHang.FirstOrDefault(sp =>
                sp.SanPhamId == giohang.SanPhamId &&
                sp.ApplicationUserId == giohang.ApplicationUserId &&
                sp.Size == giohang.Size); // Check for the same size

            if (giohangdb == null)
            {
                // Khởi tạo lại đối tượng GioHang mà không gán SecondaryImages
                var newGioHang = new GioHang()
                {
                    SanPhamId = giohang.SanPhamId,
                    Quantity = giohang.Quantity,
                    Size = giohang.Size,
                    ApplicationUserId = giohang.ApplicationUserId
                };

                _db.GioHang.Add(newGioHang);
            }
            else
            {
                giohangdb.Quantity += giohang.Quantity;
            }

            // Save changes to the database
            _db.SaveChanges();

            // Hiển thị SweetAlert 2 thông báo sau khi thêm sản phẩm vào giỏ hàng
            var script = "<script type='text/javascript'>" +
                         "Swal.fire('Added to Cart', 'The product has been added to your cart.', 'success');" +
                         "</script>";
            ViewBag.Script = script;

            return RedirectToAction("Index");
        }














        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}