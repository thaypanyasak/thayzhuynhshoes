using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsLockedOut { get; set; }
    }
}
