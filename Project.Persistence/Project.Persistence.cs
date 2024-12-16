
using System.Text.Json;
using Project.Models;

namespace Project.Persistence
{
    public class OrderPersistence
{
    private const string FilePath = "orders.json";

    public void SaveOrder(Order order)
    {
        if (order.Items.Count == 0) 
        {
            return;
        }

        var orders = LoadOrders();
        orders.Add(order);
        SaveAllOrders(orders);
    }

    public List<Order> LoadOrders()
    {
        if (!File.Exists(FilePath))
        {
            return new List<Order>();
        }

        string json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<Order>>(json) ?? new List<Order>();
    }

    
    public void SaveAllOrders(List<Order> orders)
    {
        string json = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }
}

}



