namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
