namespace MiniCartMvc.ViewModels
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double Price { get; set; }
        public int Stock { get; set; }
        public string? Image { get; set; }
        public double AverageRating { get; set; }


        // Yorumlar
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
    }
}
