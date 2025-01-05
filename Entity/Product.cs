using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniCartMvc.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        [DisplayName("Açıklama")]
        public string Description { get; set; } = "";
        public double Price { get; set; }
        public int Stock { get; set; }
        public string? ImagePath { get; set; }
        public bool IsApproved { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        public List<Comment>? Comments { get; set; } = new List<Comment>();

    }
}
