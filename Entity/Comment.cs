using MiniCartMvc.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniCartMvc.Entity
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public string UserId { get; set; } = "";

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
