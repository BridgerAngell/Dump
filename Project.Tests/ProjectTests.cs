using System;
using System.IO;
using System.Linq;
using Project.Logic;
using Project.Models;

namespace Project.Tests
{
    public class OrderServiceTests : IDisposable
    {
        private readonly OrderService _orderService;
        private const string OrdersFile = "orders.json";

        public OrderServiceTests()
        {

            if (File.Exists(OrdersFile))
            {
                File.Delete(OrdersFile);
            }

            _orderService = new OrderService(); // (REQ#1.1.3, 1.2.3, 1.3.3, 1.4.3, 1.5.3, 1.6.3, 1.7.3)
        }

        public void Dispose()
        {
            
            if (File.Exists(OrdersFile))
            {
                File.Delete(OrdersFile);
            }
        }

        #region Feature 1: Multiple Quantities of an Item
        [Fact] 
        // (REQ#1.1.1) 
        public void AddItemToOrder_MultipleQuantities_Success()
        {
            var order = new Order();
            var result = _orderService.AddItemToOrder(order, 2, 3); 
            Assert.Contains("added", result, StringComparison.OrdinalIgnoreCase);
            Assert.Single(order.Items);
            Assert.Equal(3, order.Items.First().Quantity);
        }

        [Fact]
        // (REQ#1.1.2) 
        public void AddItemToOrder_InvalidQuantity_Failure()
        {
            var order = new Order();
            var result = _orderService.AddItemToOrder(order, 2, 0);
            Assert.Contains("Quantity must be greater than zero", result, StringComparison.OrdinalIgnoreCase);
            Assert.Empty(order.Items);
        }
        #endregion

        #region Feature 2: Multiple Different Types of Items

        [Fact]
        // (REQ#1.2.1) 
        public void AddMultipleDifferentItems_Success()
        {
            var order = new Order();
            _orderService.AddItemToOrder(order, 1, 2);
            _orderService.AddItemToOrder(order, 2, 1);

            Assert.Equal(2, order.Items.Count);
            Assert.Equal(2, order.Items.First(i => i.Product.Id == 1).Quantity);
            Assert.Equal(1, order.Items.First(i => i.Product.Id == 2).Quantity);
        }

        [Fact]
        // (REQ#1.2.2) 
        public void AddMultipleDifferentItems_InvalidProductId_Failure()
        {
            var order = new Order();
            var result = _orderService.AddItemToOrder(order, 999, 1);
            Assert.Contains("Invalid product ID", result, StringComparison.OrdinalIgnoreCase);
            Assert.Empty(order.Items);
        }
        #endregion

        #region Feature 3: Sales Tax Calculation

        [Fact]
        // (REQ#1.3.1) 
        public void SalesTaxCalculation_Success()
        {
            var order = new Order();
            _orderService.AddItemToOrder(order, 1, 2);
            Assert.Equal(5.00m, order.Subtotal);
            Assert.Equal(0.50m, order.SalesTax);
            Assert.Equal(5.50m, order.Total);
        }

        [Fact]

        // (REQ#1.3.2) 
        public void SalesTaxCalculation_EmptyOrder_Failure()
        {
            var order = new Order();
            Assert.Equal(0m, order.Subtotal);
            Assert.Equal(0m, order.SalesTax);
            Assert.Equal(0m, order.Total);

            var finalizeResult = _orderService.FinalizeOrder(order);
            Assert.Contains("Cannot finalize an empty order", finalizeResult, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region Feature 4: Finalize Order Visible in Pending Orders

        [Fact]
        // (REQ#1.4.1) 
        public void FinalizeOrder_SavesAndShowsPending_Success()
        {
            var order = new Order();
            _orderService.AddItemToOrder(order, 1, 1);
            _orderService.FinalizeOrder(order);

            var pending = _orderService.GetPendingOrders();
            Assert.Single(pending);
            var pendingOrder = pending.First();
            Assert.Equal(order.Items.Count, pendingOrder.Items.Count);
            Assert.Equal(order.Subtotal, pendingOrder.Subtotal);
            Assert.Equal(order.IsFulfilled, pendingOrder.IsFulfilled);
        }

        [Fact]
        // (REQ#1.4.2) 
        public void FinalizeOrder_EmptyOrder_Failure()
        {
            var order = new Order();
            var result = _orderService.FinalizeOrder(order);
            Assert.Contains("Cannot finalize an empty order", result, StringComparison.OrdinalIgnoreCase);
            Assert.Empty(_orderService.GetPendingOrders());
        }
        #endregion

        #region Feature 5: Workers Fulfill Orders

        [Fact]
        // (REQ#1.5.1) 
        public void FulfillOrder_Success()
        {
            var order = new Order();
            _orderService.AddItemToOrder(order, 1, 1);
            _orderService.FinalizeOrder(order);

            var fulfillResult = _orderService.FulfillOrder(0);
            Assert.Contains("has been marked as fulfilled", fulfillResult, StringComparison.OrdinalIgnoreCase);

            var updatedOrders = _orderService.GetAllOrders();
            Assert.True(updatedOrders[0].IsFulfilled);
        }

        [Fact]
        // (REQ#1.5.2) 
        public void FulfillOrder_InvalidIndex_Failure()
        {
            var fulfillResult = _orderService.FulfillOrder(999);
            Assert.Contains("Invalid order", fulfillResult, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region Feature 6: Manager Day Summary

        [Fact]
        // (REQ#1.6.1) 
        public void GetOrdersForDay_Success()
        {
            var order = new Order { OrderDay = DayOfWeek.Monday };
            _orderService.AddItemToOrder(order, 1, 2);
            _orderService.FinalizeOrder(order);

            var mondayOrders = _orderService.GetOrdersForDay(DayOfWeek.Monday);
            Assert.Single(mondayOrders);

            var fetchedOrder = mondayOrders.First();
            Assert.Equal(order.Items.Count, fetchedOrder.Items.Count);
            Assert.Equal(order.Subtotal, fetchedOrder.Subtotal);
            Assert.Equal(order.OrderDay, fetchedOrder.OrderDay);
        }

        [Fact]
        // (REQ#1.6.2) Failure path
        public void GetOrdersForDay_NoOrders_Failure()
        {
            var tuesdayOrders = _orderService.GetOrdersForDay(DayOfWeek.Tuesday);
            Assert.Empty(tuesdayOrders);
        }
        #endregion

        #region Feature 7: Manager Can See Details of Any Individual Order


        [Fact]
        // (REQ#1.7.1) Happy path
        public void GetOrderDetails_Success()
        {
            var order = new Order();
            _orderService.AddItemToOrder(order, 1, 1);
            _orderService.FinalizeOrder(order);

            var allOrders = _orderService.GetAllOrders();
            var firstOrder = allOrders[0];
            Assert.NotNull(firstOrder);
            Assert.Single(firstOrder.Items);
        }

        [Fact]
        // (REQ#1.7.2) Failure path
        public void GetOrderDetails_InvalidIndex_Failure()
        {
            var allOrders = _orderService.GetAllOrders();
            Assert.Empty(allOrders);
            Assert.Throws<ArgumentOutOfRangeException>(() => { var o = allOrders[0]; });
        }
        #endregion



        //(REQ#1.8.1)
        #region Manager screen password

        [Fact] // Happy Path
        public void ManagerAccess_CorrectPassword_Success()
        {
            var canAccess = _orderService.CheckManagerPassword("manager123");
            Assert.True(canAccess);
        }


        //(REQ#1.8.1)

        [Fact] // Failure Path
        public void ManagerAccess_IncorrectPassword_Failure()
        {
            var canAccess = _orderService.CheckManagerPassword("wrongpass");
            Assert.False(canAccess);
        }
        #endregion
    }
}
