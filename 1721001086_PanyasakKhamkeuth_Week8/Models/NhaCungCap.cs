using System.ComponentModel.DataAnnotations;

namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class NhaCungCap
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public int PhoneNumber { get; set; }


    }
}
