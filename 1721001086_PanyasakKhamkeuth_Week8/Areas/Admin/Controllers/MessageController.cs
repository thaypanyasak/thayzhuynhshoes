using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MessageController(ApplicationDbContext db)
        {
            _db = db;
            
        }
        public ActionResult Index()
        {
            var messages = _db.Messages.ToList(); // Lấy danh sách tin nhắn từ cơ sở dữ liệu
            return View(messages);
        }


        [HttpPost]
        public ActionResult Index(Message message)
        {
            if (ModelState.IsValid)
            {
                // Xử lý tin nhắn ở đây
                var contactMessage = new Message
                {
                    Name = message.Name,
                    Email = message.Email,
                    Messages = message.Messages,
                    CreatedAt = DateTime.Now
                };

                _db.Messages.Add(contactMessage);
                _db.SaveChanges();

                // Sau khi lưu thông tin liên hệ thành công, bạn có thể chuyển hướng hoặc hiển thị thông báo thành công
                return RedirectToAction("Index");
            }
            return View(message);
        }


        
    }
}
