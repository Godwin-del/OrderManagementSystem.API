using OrderManagementSystem.Application.Rules;
using OrderManagementSystem.Application.Services;
using OrderManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrderManagementSystem.Tests.Unit
{
    public class DiscountServiceTests
    {
        private DiscountService CreateDiscountService(IEnumerable<PromotionRule> rules) =>
            new DiscountService(rules);

        [Fact]
        public void CalculateOrderDiscount_GoldCustomer_ShouldApply10Percent()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Segment = "Gold",
                Orders = new List<Order>()
            };
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Customer = customer,
                Items = new List<OrderItem>
                {
                    new() { Quantity = 2, UnitPrice = 100m }
                }
            };
            customer.Orders.Add(order);

            var rules = new PromotionRule[] { new GoldCustomerRule() };
            var svc = CreateDiscountService(rules);

            // Act
            var result = svc.CalculateOrderDiscount(order);

            // Assert
            Assert.Equal(200m, result.OriginalTotal);
            Assert.Equal(20m, result.DiscountAmount);   // 10% of 200
            Assert.Contains("Gold Customer 10% Off", result.AppliedRules);
        }

        [Fact]
        public void CalculateOrderDiscount_LoyalCustomer_ShouldApply5Percent()
        {
            // Arrange: Customer with 5+ past orders in last year
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Segment = "Silver",
                Orders = new List<Order>()
            };
            // Create 5 past orders (different IDs)
            for (int i = 0; i < 5; i++)
            {
                customer.Orders.Add(new Order
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow.AddMonths(-2),
                    Customer = customer,
                    Items = new List<OrderItem> { new() { Quantity = 1, UnitPrice = 50m } }
                });
            }
            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                Customer = customer,
                Items = new List<OrderItem> { new() { Quantity = 4, UnitPrice = 100m } },
                CreatedAt = DateTime.UtcNow
            };
            customer.Orders.Add(newOrder);

            var rules = new PromotionRule[] { new LoyalCustomerRule() };
            var svc = CreateDiscountService(rules);

            // Act
            var result = svc.CalculateOrderDiscount(newOrder);

            // Assert: total = 400, 5% off = 20
            Assert.Equal(400m, result.OriginalTotal);
            Assert.Equal(20m, result.DiscountAmount);
            Assert.Contains("Loyal Customer 5% Off", result.AppliedRules);
        }

        [Fact]
        public void CalculateOrderDiscount_HighValueOrder_ShouldApply25Flat()
        {
            // Arrange: Single order total 600
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Customer = new Customer { Id = Guid.NewGuid(), Segment = "Bronze" },
                Items = new List<OrderItem> { new() { Quantity = 6, UnitPrice = 100m } }
            };

            var rules = new PromotionRule[] { new HighValueOrderRule() };
            var svc = CreateDiscountService(rules);

            // Act
            var result = svc.CalculateOrderDiscount(order);

            // Assert
            Assert.Equal(600m, result.OriginalTotal);
            Assert.Equal(25m, result.DiscountAmount);
            Assert.Contains("High-Value Order $25 Off", result.AppliedRules);
        }

        [Fact]
        public void CalculateOrderDiscount_CombinationRules_ShouldSumAll()
        {
            // Arrange: Gold customer with total 1000, 5 past orders
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Segment = "Gold",
                Orders = new List<Order>()
            };
            for (int i = 0; i < 5; i++)
            {
                customer.Orders.Add(new Order
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow.AddMonths(-2),
                    Customer = customer,
                    Items = new List<OrderItem> { new() { Quantity = 1, UnitPrice = 100m } }
                });
            }
            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                Customer = customer,
                Items = new List<OrderItem> { new() { Quantity = 10, UnitPrice = 100m } },
                CreatedAt = DateTime.UtcNow
            };
            customer.Orders.Add(newOrder);

            var rules = new PromotionRule[]
            {
                new GoldCustomerRule(),
                new LoyalCustomerRule(),
                new HighValueOrderRule()
            };
            var svc = CreateDiscountService(rules);

            // Act
            var result = svc.CalculateOrderDiscount(newOrder);

            // Expected:
            // 1) 10% of 1000 = 100
            // 2) 5% of 1000 = 50
            // 3) flat 25
            // Total discount = 175
            Assert.Equal(1000m, result.OriginalTotal);
            Assert.Equal(175m, result.DiscountAmount);
            Assert.Contains("Gold Customer 10% Off", result.AppliedRules);
            Assert.Contains("Loyal Customer 5% Off", result.AppliedRules);
            Assert.Contains("High-Value Order $25 Off", result.AppliedRules);
        }
    }
}
