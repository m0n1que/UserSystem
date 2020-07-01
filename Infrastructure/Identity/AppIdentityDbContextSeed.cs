using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "mojomi", 
                    Email = "monica@test.com", 
                    UserName = "monica@test.com", 
                    FirstName = "Monika",
                    LastName = "Jovanova Mingova", 
                    CompanyName = "KOGTEK"
                };
                await userManager.CreateAsync(user, "Ne$ka%ee2");
            }
        }
    }
}