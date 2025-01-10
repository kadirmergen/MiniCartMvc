using System.ComponentModel.DataAnnotations;

namespace MiniCartMvc.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public double Total { get; set; }
        public DateTime OrderDate { get; set; }
        public EnumOrderState OrderState {  get; set; } 

        public string UserName { get; set; }

        public string AddressTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Strict { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }

        public virtual List<OrderLine> OrderLines { get; set; }
    }

    public class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; } //ürünün alındığı zamandaki fiyatını görebilmek için
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
