using System.Threading.Tasks;
using _1721001086_PanyasakKhamkeuth_Week8.Models;

namespace _1721001086_PanyasakKhamkeuth_Week8.Services
{
    public class ChatService : IChatService
    {
        public string SendMessageToUser(ChatMessage message)
        {
            // ทำการส่งข้อความไปยังผู้ใช้ และคืนข้อความตอบกลับ
            return "ข้อความถูกส่งสำเร็จ";
        }
        public string SendMessageToAdmin(ChatMessage message)
        {
            // ทำการส่งข้อความไปยังผู้ใช้ และคืนข้อความตอบกลับ
            return "ข้อความถูกส่งสำเร็จ";
        }
    }
}
