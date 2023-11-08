using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using System.Globalization;
using static Google.Cloud.Dialogflow.V2.Intent.Types.Message.Types.CarouselSelect.Types;
using static Google.Api.ResourceDescriptor.Types;
using System.Runtime.InteropServices;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Customer.Controllers
{
    [Area("User")]
    public class GioHangController : Controller
    {
        private IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        public GioHangController(ApplicationDbContext db, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize]
        public IActionResult Index()
        {

            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang.Include("SanPham").Where(gh => gh.ApplicationUserId == claim.Value).ToList(),
                HoaDon = new HoaDon()
            };

            if (giohang.HoaDon != null && giohang.HoaDon.ApplicationUser != null)
            {
                giohang.HoaDon.Name = giohang.HoaDon.ApplicationUser.Name;
                giohang.HoaDon.Address = giohang.HoaDon.ApplicationUser.Address;
                giohang.HoaDon.PhoneNumber = giohang.HoaDon.ApplicationUser.PhoneNumber;
            }

            giohang.ShippingFee = giohang.DsGioHang.Any() ? 30000.0 : 0.0;
            giohang.TaxAmount = giohang.DsGioHang.Any() ? 0.1 : 0.0;

            double totalProductPrice = 0.0;

            foreach (var item in giohang.DsGioHang)
            {
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                totalProductPrice += item.ProductPrice;
            }

            giohang.TaxAmount = totalProductPrice * giohang.TaxAmount;
            giohang.HoaDon.Total = totalProductPrice + giohang.ShippingFee + giohang.TaxAmount;
            giohang.TotalPrice = giohang.HoaDon.Total;

            return View(giohang);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThanhToan(GioHangViewModel giohang)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            giohang.DsGioHang = _db.GioHang.Include("SanPham")
                .Where(gh => gh.ApplicationUserId == claim.Value).ToList();

            if (giohang.HoaDon == null)
                giohang.HoaDon = new HoaDon();

            giohang.HoaDon.ApplicationUserId = claim.Value;
            giohang.HoaDon.OrderDate = DateTime.Now;
            giohang.HoaDon.OrderStatus = "Đang xác nhận";

            // Tạo mã ngẫu nhiên cho "Order No"
            string orderNumber = RandomCodeGenerator.GenerateRandomCode(8);
            giohang.HoaDon.OrderNumber = orderNumber;

            // Tính TotalPrice dựa trên giỏ hàng
            giohang.ShippingFee = giohang.DsGioHang.Any() ? 30000.0 : 0.0; // Nếu có sản phẩm thì ShippingFee = 30000, ngược lại là 0
            giohang.TaxAmount = giohang.DsGioHang.Any() ? 0.1 : 0.0; // Nếu có sản phẩm thì TaxAmount = 0.1, ngược lại là 0

            double totalProductPrice = 0.0; // Initialize total product price

            foreach (var item in giohang.DsGioHang)
            {
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                totalProductPrice += item.ProductPrice; // Add each product's price to the total product price
            }

            // Calculate the tax based on the total product price
            giohang.TaxAmount = totalProductPrice * giohang.TaxAmount;

            // Calculate the total including tax and shipping fee
            giohang.HoaDon.Total = totalProductPrice + giohang.ShippingFee + giohang.TaxAmount;
            giohang.TotalPrice = giohang.HoaDon.Total; // TotalPrice doesn't need to include tax and shipping fee

            // Serialize and store the GioHangViewModel object in the session
            var giohangData = JsonConvert.SerializeObject(giohang);
            HttpContext.Session.SetString("TempGioHang", giohangData);

            // Thêm đơn hàng vào cơ sở dữ liệu
            _db.HoaDon.Add(giohang.HoaDon);
            _db.SaveChanges();

            // Thêm các chi tiết đơn hàng vào cơ sở dữ liệu
            foreach (var item in giohang.DsGioHang)
            {
                ChiTietHoaDon chitiethoadon = new ChiTietHoaDon()
                {
                    SanPhamId = item.SanPhamId,
                    HoaDonId = giohang.HoaDon.Id,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                };
                _db.ChiTietHoaDon.Add(chitiethoadon);
            }

            // Xóa các sản phẩm khỏi giỏ hàng
            _db.GioHang.RemoveRange(giohang.DsGioHang);
            _db.SaveChanges();
            SendConfirmationEmail(giohang.HoaDon);
            // Chuyển hướng đến trang Success
            return RedirectToAction("Success", "GioHang");
        }
        private void SendConfirmationEmail(HoaDon hoaDon)
        {
            if (hoaDon != null && !string.IsNullOrEmpty(hoaDon.Email))
            {
                var email = hoaDon.Email;


                string eBody = $@"
<!DOCTYPE html>
<html>
<head>
</head>
<body>
    <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
        <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; border: 1px solid #ccc; border-radius: 5px;'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src='https://blogger.googleusercontent.com/img/a/AVvXsEhvDFmSY6Ag3G5pIsQAPvpZkzTXhelX9ih403tp4Pwpa-29tAVXbi9CzQI3xnKvMer9xOQiwkWv2tsKK3sSS8MojvcYw3b9PHK1lR4P3qIGTHRxcJWNJkMxou7nQJqGcB0EPDm49CEAuwjrnO2CYJafnf0ZimIt5j81K_yd97r3mxfysJtAaN6Jh8KL-c8' alt='Logo cửa hàng' style='max-width: 150px;'>
            </div>
            <h1 style='color: #333; text-align: center;'>Xác nhận đơn hàng</h1>
            <p style='font-size: 16px; color: #666; text-align: center;'>
                Chào bạn,
            </p>
            <p style='font-size: 16px; color: #666; text-align: center;'>
                Cám ơn bạn đã đặt hàng tại cửa hàng giày của chúng tôi. Đơn hàng của bạn đã được xác nhận thành công!
            </p>
            <p style='font-size: 16px; color: #666; text-align: left;'>
                Thông tin đơn hàng:
            </p>
            <ul style='list-style-type: none; padding: 0; text-align: left;'>
                <li style='font-size: 16px; color: #666; margin: 10px 0;'>
                    <strong>&#10004; Mã đơn hàng:</strong> {hoaDon.OrderNumber}
                </li>
                <li style='font-size: 16px; color: #666; margin: 10px 0;'>
                    <strong>&#128100; Tên khách hàng:</strong> {hoaDon.Name}
                </li>
                <li style='font-size: 16px; color: #666; margin: 10px 0;'>
                    <strong>&#128181; Tổng tiền:</strong> {hoaDon.Total.ToString("#,##0")} VND
                </li>
            </ul>
            <p style='font-size: 16px; color: #666; text-align: center;'>
                Cảm ơn bạn đã ủng hộ cửa hàng của chúng tôi. Chúng tôi sẽ liên hệ với bạn để xác nhận và giao hàng trong thời gian sớm nhất.
            </p>
            <p style='font-size: 16px; color: #666; text-align: center;'>Trân trọng, Cảm ơn</p>
            <div style='text-align: center; margin-top: 20px;'>
                <hr>
                <p style='font-size: 14px; color: #999;'>BY: PANYASAK KHAMKEUTH - Nguyễn Nhật Huỳnh</p>
                <hr>
            </div>
        </div>
    </div>
</body>
</html>
";











                MailMessage message = new MailMessage(new MailAddress("Khamkerd4000@gmail.com", "Confirm your email"), new MailAddress(email));
                message.Subject = "Order Success";
                message.Body = eBody;
                message.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("khamkerd4000@gmail.com", "kymibagucjfltrib");
                smtp.EnableSsl = true;

                smtp.Send(message);
            }
        }







        public IActionResult Tang(int giohangId)
        {

            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            giohang.Quantity += 1;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Giam(int giohangId)
        {
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);

            giohang.Quantity -= 1;

            if (giohang.Quantity == 0)
            {
                _db.GioHang.Remove(giohang);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Xoa(int giohangId)
        {
            
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            if (giohang != null)
            {
                _db.GioHang.Remove(giohang);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
        }


        [Authorize]
        public IActionResult ThanhToan()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang.Include("SanPham").Where(gh => gh.ApplicationUserId == claim.Value).ToList(),
                HoaDon = new HoaDon()
            };


            giohang.ShippingFee = giohang.DsGioHang.Any() ? 30000.0 : 0.0; // Nếu có sản phẩm thì ShippingFee = 30000, ngược lại là 0
            giohang.TaxAmount = giohang.DsGioHang.Any() ? 0.1 : 0.0; // Nếu có sản phẩm thì TaxAmount = 0.1, ngược lại là 0

            double totalProductPrice = 0.0; // Initialize total product price

            foreach (var item in giohang.DsGioHang)
            {
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                totalProductPrice += item.ProductPrice; // Add each product's price to the total product price
            }

            // Calculate the tax based on the total product price
            giohang.TaxAmount = totalProductPrice * giohang.TaxAmount;

            // Calculate the total including tax and shipping fee
            giohang.HoaDon.Total = totalProductPrice + giohang.ShippingFee + giohang.TaxAmount;
            giohang.TotalPrice = giohang.HoaDon.Total; // TotalPrice doesn't need to include tax and shipping fee

            return View(giohang);
        }

        public IActionResult Success()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            // Retrieve the serialized GioHangViewModel object from the session
            var giohangData = HttpContext.Session.GetString("TempGioHang");

            if (!string.IsNullOrEmpty(giohangData))
            {
                // Deserialize the GioHangViewModel object
                var giohang = JsonConvert.DeserializeObject<GioHangViewModel>(giohangData);

                // Now you can use 'giohang' as the deserialized object
                // Rest of your Success action logic to display order information
                return View(giohang);
            }

            // Handle the case when giohangData is empty or not found in the session

            return View();

        }
        public class RandomCodeGenerator
        {
            private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            public static string GenerateRandomCode(int length)
            {
                Random random = new Random();
                StringBuilder code = new StringBuilder(length);

                for (int i = 0; i < length; i++)
                {
                    int index = random.Next(Characters.Length);
                    code.Append(Characters[index]);
                }

                return code.ToString();
            }
        }

    }
}
