namespace MiniCartMvc.ViewModels
{
    public class CommentViewModel
    {
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; } = ""; 
    }
}
