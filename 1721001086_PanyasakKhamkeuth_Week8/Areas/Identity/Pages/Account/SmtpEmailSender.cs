using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using _1721001086_PanyasakKhamkeuth_Week8.Models;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Identity.Pages.Account
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpEmailSenderOptions _emailSenderOptions;

        public SmtpEmailSender(IOptions<SmtpEmailSenderOptions> emailSenderOptions)
        {
            _emailSenderOptions = emailSenderOptions.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient
            {
                Host = _emailSenderOptions.SmtpServer,
                Port = _emailSenderOptions.SmtpPort,
                EnableSsl = true, // Enable SSL if required
                Credentials = new NetworkCredential(_emailSenderOptions.SmtpUsername, _emailSenderOptions.SmtpPassword) // ใช้ชื่อผู้ใช้ Gmail เป็น SmtpUsername และรหัสแอปพลิเคชันที่คุณสร้างเป็น SmtpPassword
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSenderOptions.FromEmail, _emailSenderOptions.FromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}
