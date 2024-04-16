using Microsoft.AspNetCore.Identity;

namespace file_managment.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
