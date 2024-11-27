using Microsoft.AspNetCore.Identity;

namespace MiniCartMvc.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
