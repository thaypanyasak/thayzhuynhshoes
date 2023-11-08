using _1721001086_PanyasakKhamkeuth_Week8.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace _1721001086_PanyasakKhamkeuth_Week8.ViewComponents
{
	public class TheLoaiViewComponent : ViewComponent
	{
		public readonly ApplicationDbContext _db;
		public TheLoaiViewComponent(ApplicationDbContext db)
		{
			_db = db;
		}
		public IViewComponentResult Invoke()
		{
			var theloai = _db.TheLoai.ToList();
			return View(theloai);
		}
	}
}
