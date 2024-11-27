using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MiniCartMvc.Models
{
    public class RegisterModel
    {
        //what we want from users
        [Required]
        [DisplayName("First Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string Surname { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DisplayName("E-Mail")]
        [EmailAddress(ErrorMessage = "This e-mail address does not in a correct format")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [DisplayName("Password Confirmation")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string RePassword { get; set; }

    }
}
