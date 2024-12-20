using System.ComponentModel.DataAnnotations;

namespace MiniCartMvc.Models
{
    public class ShippingDetail
    {
        public string UserName { get; set; }

        [Required(ErrorMessage ="Please enter the Adress Header.")]
        public string AddressTitle { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        public string City { get; set; }
        public string Strict { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }

    }
}
