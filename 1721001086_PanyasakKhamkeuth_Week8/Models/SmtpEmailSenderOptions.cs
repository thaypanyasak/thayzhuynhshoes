namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class SmtpEmailSenderOptions
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; } // เพิ่มคุณสมบัติรหัสแอปพลิเคชัน
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
