using MiniCartMvc.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniCartMvc.Entity
{
    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public string UserId { get; set; } = "";
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
