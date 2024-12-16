using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Project.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class OrderItem
    {
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }

        [JsonIgnore] 
        public decimal LineTotal => Product.Price * Quantity;
    }

 public class Order
    {
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        
        
        public DateTime OrderDate { get; set; }
        public DayOfWeek OrderDay { get; set; }
        
      
        public decimal Subtotal => Items.Sum(item => item.LineTotal);
        public decimal SalesTax => Subtotal * 0.1m;
        public decimal Total => Subtotal + SalesTax;
        public bool IsFulfilled { get; set; } = false;
    }



}
