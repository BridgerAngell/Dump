using Project.Logic;
using Project.Models;

namespace Project.UI
{
    class Program
    {
        static void Main()
        {
            var orderService = new OrderService();
            var order = new Order();

            //(REQ#1.6.3)
            var random = new Random();
            int randomNumber = random.Next(1, 8); 
            DayOfWeek currentDay = (randomNumber == 7) ? DayOfWeek.Sunday : (DayOfWeek)randomNumber;

            Console.WriteLine("Welcome to the 2019-Prices Cafe!");
            Console.WriteLine($"Today is {currentDay}.");

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add Item to Order"); 
                Console.WriteLine("2. Finalize Order"); 
                Console.WriteLine("3. View Pending Orders"); 
                Console.WriteLine("4. Worker: Fulfill an Order"); 
                Console.WriteLine("5. Manager: View One-Line Summary for a Given Day of the Week"); 
                Console.WriteLine("6. Exit");
                Console.Write("Choose an option: ");
                var input = Console.ReadLine();

                if (input == "6")
                    break;

                switch (input)
                {
                    case "1":
                        //(REQ#1.1.3, 1.2.3)
                        Console.WriteLine("\nAvailable Products:");
                        foreach (var product in orderService.GetAvailableProducts())
                        {
                            Console.WriteLine($"{product.Id}. {product.Name} - ${product.Price}");
                        }

                        Console.Write("Enter product ID: ");
                        var productIdInput = Console.ReadLine();

                        Console.Write("Enter quantity: ");
                        var quantityInput = Console.ReadLine();

                        // 
                        var result = orderService.ProcessOrderInput(order, productIdInput!, quantityInput!);
                        Console.WriteLine(result);
                        break;

                    case "2":
                        // (REQ#1.4.3)
                        // (REQ#1.3.3 )
                        Console.WriteLine("\nFinalizing Order...");
                        Console.WriteLine($"Subtotal: ${order.Subtotal}");
                        Console.WriteLine($"Sales Tax: ${order.SalesTax}");
                        Console.WriteLine($"Total: ${order.Total}");

                        order.OrderDay = currentDay;
                    
                        var finalizeResult = orderService.FinalizeOrder(order);
                        Console.WriteLine(finalizeResult);
                        order = new Order(); 
                        break;

                    case "3":
                        // (REQ#1.4.3)
                        Console.WriteLine("\nPending Orders:");
                        var pendingOrders = orderService.GetPendingOrders();
                        if (pendingOrders.Count == 0)
                        {
                            Console.WriteLine("No pending orders.");
                        }
                        else
                        {
                            int index = 1;
                            foreach (var pendingOrder in pendingOrders)
                            {
                                Console.WriteLine($"\nOrder #{index}:");
                                foreach (var item in pendingOrder.Items)
                                {
                                    
                                    Console.WriteLine($"{item.Quantity} x {item.Product.Name} - ${item.LineTotal}");
                                }
                                // (REQ#1.3.3)
                                Console.WriteLine($"Subtotal: ${pendingOrder.Subtotal}");
                                Console.WriteLine($"Sales Tax: ${pendingOrder.SalesTax}");
                                Console.WriteLine($"Total: ${pendingOrder.Total}");
                                Console.WriteLine($"Fulfilled: {pendingOrder.IsFulfilled}");
                                Console.WriteLine($"Day: {pendingOrder.OrderDay}");
                                index++;
                            }
                        }
                        break;

                    case "4":
                        // (REQ#1.5.3)
                        // (REQ#1.7.3)
                        Console.WriteLine("\nAll Orders:");
                        var allOrders = orderService.GetAllOrders();
                        if (allOrders.Count == 0)
                        {
                            Console.WriteLine("No orders found.");
                        }
                        else
                        {
                            for (int i = 0; i < allOrders.Count; i++)
                            {
                                var o = allOrders[i];
                                Console.WriteLine($"\nOrder #{i+1}:");
                                foreach (var item in o.Items)
                                {
                                    Console.WriteLine($"{item.Quantity} x {item.Product.Name} - ${item.LineTotal}");
                                }
                                Console.WriteLine($"Subtotal: ${o.Subtotal}");
                                Console.WriteLine($"Sales Tax: ${o.SalesTax}");
                                Console.WriteLine($"Total: ${o.Total:F2}");
                                Console.WriteLine($"Fulfilled: {o.IsFulfilled}");
                                Console.WriteLine($"Day: {o.OrderDay}");
                            }

                            Console.Write("\nEnter the order number you want to fulfill: ");
                            var fulfillInput = Console.ReadLine();
                            if (int.TryParse(fulfillInput, out int fulfillIndex))
                            {
                                
                                string fulfillResult = orderService.FulfillOrder(fulfillIndex - 1);
                                Console.WriteLine(fulfillResult);
                            }
                            else
                            {
                                Console.WriteLine("Invalid input.");
                            }
                        }
                        break;

                    case "5":
                        
                        Console.Write("Enter manager password: ");
                        var password = Console.ReadLine();
                        // FREQ#1.8.3)
                        if (password != "uh")
                        {
                            Console.WriteLine("Incorrect password. Access denied.");
                            break;
                        }

                        Console.WriteLine("\nSelect a day of the week (1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday, 7=Sunday): ");
                        var dayInput = Console.ReadLine();
                        if (int.TryParse(dayInput, out int dayChoice) && dayChoice >= 1 && dayChoice <= 7)
                        {
                            DayOfWeek selectedDay = (dayChoice == 7) ? DayOfWeek.Sunday : (DayOfWeek)dayChoice;

                            var ordersForDay = orderService.GetOrdersForDay(selectedDay);
                           
                            if (ordersForDay.Count == 0)
                            {
                                Console.WriteLine($"No orders found for {selectedDay}.");
                            }
                            else
                            {
                                Console.WriteLine($"\nOrders for {selectedDay}:");
                                for (int i = 0; i < ordersForDay.Count; i++)
                                {
                                    var dayOrder = ordersForDay[i];
                                    int totalItems = dayOrder.Items.Sum(item => item.Quantity);
                                    //(REQ#1.6.3)
                                    Console.WriteLine($"Order #{i+1}: {totalItems} items, Total: ${dayOrder.Total:F2}, Fulfilled: {dayOrder.IsFulfilled}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Choose a number between 1 and 7.");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }

            Console.WriteLine("Come again soon!");
        }
    }
}
