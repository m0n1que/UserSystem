using Microsoft.AspNetCore.Identity;

namespace Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
    }
}