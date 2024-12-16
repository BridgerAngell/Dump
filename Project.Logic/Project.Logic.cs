
using Project.Persistence;
using Project.Models;

namespace Project.Logic
{
    public class OrderService
    {
    
        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Coffee", Price = 2.50m },
            new Product { Id = 2, Name = "Black Tea", Price = 1.00m },
            new Product { Id = 3, Name = "Matcha Tea", Price = 2.00m },
            new Product { Id = 4, Name = "Yerba Mate", Price = 1.50m },
            new Product { Id = 5, Name = "Hot Chocolate Delux", Price = 8.00m },
            new Product { Id = 6, Name = "Matcha Shake Delux", Price = 12.00m }
        };

        private readonly OrderPersistence _persistence = new();

        public List<Product> GetAvailableProducts()
        {
          
            return _products;
        }

        public string ProcessOrderInput(Order order, string productIdInput, string quantityInput)
        {
            
            try
            {
                int productId = ParseInteger(productIdInput);     // (REQ#1.1.3 & REQ#1.2.3)
                int quantity = ParseInteger(quantityInput);

                if (quantity <= 0)
                {
                   
                    return "Please enter a quantity greater than 0.";
                }

                var product = GetProductById(productId);
                if (product == null)
                {
                    
                    return "Invalid product ID. Please try again.";
                }

                // If valid, add item to order
                return AddItemToOrder(order, productId, quantity);
            }
            catch (ArgumentException ex)
            {
                return ex.Message;
            }
        }

        public string AddItemToOrder(Order order, int productId, int quantity)
        {
            // (REQ#1.1.3) Code implementing adding multiple quantities
            // (REQ#1.2.3) Code implementing adding different item types

            if (quantity <= 0)
            {
                
                return "Quantity must be greater than zero.";
            }

            var product = GetProductById(productId);
            if (product == null)
            {
                
                return "Invalid product ID. Please try again.";
            }

            
            var existingItem = order.Items.FirstOrDefault(item => item.Product.Id == product.Id);
            if (existingItem == null)
            {
                // (REQ#1.2.3)
                order.Items.Add(new OrderItem { Product = product, Quantity = quantity });
            }
            else
            {
                //(REQ#1.1.3)
                existingItem.Quantity += quantity;
            }

            return $"{quantity} x {product.Name} added to your order.";
        }

        public string FinalizeOrder(Order order)
        {
            // (REQ#1.4.3) 

            if (order.Items.Count == 0)
            {
                
                return "Cannot finalize an empty order.";
            }

            
           
            _persistence.SaveOrder(order);  //(REQ#1.4.3)
            return "Order has been finalized and saved.";
        }

        public List<Order> GetPendingOrders()
        {
            // (REQ#1.4.3)
            return _persistence.LoadOrders().Where(o => !o.IsFulfilled).ToList();
        }

        private Product? GetProductById(int productId)
        {
            //
            foreach (var product in _products)
            {
                if (product.Id == productId)
                {
                    return product;
                }
            }
            return null;
        }

        private int ParseInteger(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be null or empty.");
            }

            int result = 0;
            foreach (char c in input)
            {
                if (c < '0' || c > '9')
                {
                    throw new ArgumentException("Please enter a valid number.");
                }

                result = result * 10 + (c - '0');
            }

            return result;
        }

        public string FulfillOrder(int orderIndex)
        {
            // (REQ#1.5.3)

            var orders = _persistence.LoadOrders();
            if (orderIndex < 0 || orderIndex >= orders.Count)
            {
                
                return "Invalid order";
            }

            var order = orders[orderIndex];
            if (order.IsFulfilled)
            {
                return "Order is already fulfilled.";
            }

            
            order.IsFulfilled = true;
            _persistence.SaveAllOrders(orders);

            return $"Order #{orderIndex + 1} has been marked as fulfilled.";
        }

        public List<Order> GetOrdersForDay(DayOfWeek day)
        {
            // (REQ#1.6.3)
            return _persistence.LoadOrders().Where(o => o.OrderDay == day).ToList();
        }

        public List<Order> GetAllOrders()
        {
            //(REQ#1.7.3)
            return _persistence.LoadOrders();
        }

        public bool CheckManagerPassword(string password)
        {
            //(REQ#1.8.3)
            return password == "uh";
        }

    }
}
