using System.ComponentModel.DataAnnotations;


namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Messages { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
