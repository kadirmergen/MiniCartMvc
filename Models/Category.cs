using System.ComponentModel;

namespace MiniCartMvc.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [DisplayName("Kategori Adı")]
        public string Name { get; set; } = "";

        [DisplayName("Açıklama")]
        public string Description { get; set; } = "";
        public List<Product>? Products { get; set; }
    }
}
