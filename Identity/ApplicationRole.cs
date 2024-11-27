using Microsoft.AspNetCore.Identity;

namespace MiniCartMvc.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }//Bu role kimler sahip olabilir gibi bir açıklama
        public ApplicationRole() : base()
        {

        }
        public ApplicationRole(string roleName, string description) : base(roleName)
        {
            this.Description = description;
        }
    }
}
