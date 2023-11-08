using Microsoft.AspNetCore.Mvc.Rendering;

namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public bool IsLockedOut { get; set; }
        public IList<string> Roles { get; set; }
        public List<SelectListItem> RolesList { get; set; }


    }
}
