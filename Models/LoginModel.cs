using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MiniCartMvc.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
}
