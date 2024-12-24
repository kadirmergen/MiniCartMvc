using MiniCartMvc.Entity;

namespace MiniCartMvc.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public double Total { get; set; }
        public EnumOrderState OrderState { get; set; }
        public DateTime OrderDate { get; set; }
        public int? Count { get; set; } //sadece admin kullanacak
    }
}
