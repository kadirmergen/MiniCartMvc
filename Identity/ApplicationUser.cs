using Microsoft.AspNetCore.Identity;
using MiniCartMvc.Entity;

namespace MiniCartMvc.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<Rating>? Ratings { get; set; }
    }
}
