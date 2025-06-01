using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static readonly Guid GoldCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        public static readonly Guid GoldOrderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        public static readonly Guid LoyalCustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        public static readonly Guid LoyalPastOrder1Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        public static readonly Guid LoyalPastOrder2Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        public static readonly Guid LoyalPastOrder3Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        public static readonly Guid LoyalPastOrder4Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid LoyalPastOrder5Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid LoyalNewOrderId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        public static readonly Guid HighValueCustomerId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly Guid HighValueOrderId = Guid.Parse("55555555-5555-5555-5555-555555555555");

        public static readonly Guid AnalyticsCustomerId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        public static readonly Guid AnalyticsOrder1Id = Guid.Parse("77777777-7777-7777-7777-777777777777");
        public static readonly Guid AnalyticsOrder2Id = Guid.Parse("88888888-8888-8888-8888-888888888888");

        public static readonly Guid CancelCustomerId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        public static readonly Guid CancelOrderId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

        public static void SeedTestData(OrderManagementSystemContext db)
        {
            if (db.Customers.Any()) return;

            // 1. Gold customer, pending order (total = $200)
            var gold = new Customer
            {
                Id = GoldCustomerId,
                Segment = CustomerSegment.Gold.ToString(),
                DateJoined = DateTime.UtcNow.AddYears(-2),
                Orders = new List<Order>()
            };
            var goldOrder = new Order
            {
                Id = GoldOrderId,
                Customer = gold,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Pending,
                Items = new List<OrderItem>
                {
                    new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), ProductName = "GoldItem", Quantity = 2, UnitPrice = 100m }
                }
            };
            gold.Orders.Add(goldOrder);
            db.Customers.Add(gold);
            db.Orders.Add(goldOrder);

            // 2. Loyal customer (5 delivered in past year) + new pending
            var loyal = new Customer
            {
                Id = LoyalCustomerId,
                Segment = CustomerSegment.Silver.ToString(),
                DateJoined = DateTime.UtcNow.AddYears(-3),
                Orders = new List<Order>()
            };
            // Add customer first so EF Core tracks them
            db.Customers.Add(loyal);

            var deliveredDates = new[]
            {
                DateTime.UtcNow.AddMonths(-2),
                DateTime.UtcNow.AddMonths(-4),
                DateTime.UtcNow.AddMonths(-6),
                DateTime.UtcNow.AddMonths(-8),
                DateTime.UtcNow.AddMonths(-10)
            };
            var pastOrderIds = new[]
            {
                LoyalPastOrder1Id,
                LoyalPastOrder2Id,
                LoyalPastOrder3Id,
                LoyalPastOrder4Id,
                LoyalPastOrder5Id
            };
            for (int i = 0; i < 5; i++)
            {
                var past = new Order
                {
                    Id = pastOrderIds[i],
                    Customer = loyal,
                    CreatedAt = deliveredDates[i],
                    Status = OrderStatus.Delivered,
                    FulfilledAt = deliveredDates[i].AddHours(48),
                    Items = new List<OrderItem>
                    {
                        new() { Id = Guid.NewGuid(), ProductName = $"PastLoyal{i + 1}", Quantity = 1, UnitPrice = 50m }
                    }
                };
                loyal.Orders.Add(past);
                db.Orders.Add(past);
            }

            var loyalNew = new Order
            {
                Id = LoyalNewOrderId,
                Customer = loyal,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Items = new List<OrderItem>
                {
                    new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), ProductName = "LoyalNew", Quantity = 4, UnitPrice = 100m }
                }
            };
            loyal.Orders.Add(loyalNew);
            db.Orders.Add(loyalNew);

            // 3. High-value customer and order (total = $600)
            var highValue = new Customer
            {
                Id = HighValueCustomerId,
                Segment = CustomerSegment.Bronze.ToString(),
                DateJoined = DateTime.UtcNow.AddYears(-1),
                Orders = new List<Order>()
            };
            var highOrder = new Order
            {
                Id = HighValueOrderId,
                Customer = highValue,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Status = OrderStatus.Pending,
                Items = new List<OrderItem>
                {
                    new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), ProductName = "HighValue", Quantity = 6, UnitPrice = 100m }
                }
            };
            highValue.Orders.Add(highOrder);
            db.Customers.Add(highValue);
            db.Orders.Add(highOrder);

            // 4. Analytics customer with two delivered orders
            var analyticsCust = new Customer
            {
                Id = AnalyticsCustomerId,
                Segment = CustomerSegment.Silver.ToString(),
                DateJoined = DateTime.UtcNow.AddYears(-2),
                Orders = new List<Order>()
            };
            db.Customers.Add(analyticsCust);

            var delivered1 = new Order
            {
                Id = AnalyticsOrder1Id,
                Customer = analyticsCust,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Status = OrderStatus.Delivered,
                FulfilledAt = DateTime.UtcNow.AddDays(-2),
                Items = new List<OrderItem>
                {
                    new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), ProductName = "Analytic1", Quantity = 2, UnitPrice = 100m }
                }
            };
            var delivered2 = new Order
            {
                Id = AnalyticsOrder2Id,
                Customer = analyticsCust,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Status = OrderStatus.Delivered,
                FulfilledAt = DateTime.UtcNow.AddDays(-3),
                Items = new List<OrderItem>
                {
                    new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), ProductName = "Analytic2", Quantity = 5, UnitPrice = 100m }
                }
            };
            analyticsCust.Orders.Add(delivered1);
            analyticsCust.Orders.Add(delivered2);
            db.Orders.AddRange(delivered1, delivered2);

            // 5. Cancelled customer and order
            var cancelCust = new Customer
            {
                Id = CancelCustomerId,
                Segment = CustomerSegment.Bronze.ToString(),
                DateJoined = DateTime.UtcNow.AddYears(-1),
                Orders = new List<Order>()
            };
            db.Customers.Add(cancelCust);

            var cancelled = new Order
            {
                Id = CancelOrderId,
                Customer = cancelCust,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Cancelled,
                Items = new List<OrderItem>
                {
                    new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), ProductName = "Cancelled", Quantity = 3, UnitPrice = 50m }
                }
            };
            cancelCust.Orders.Add(cancelled);
            db.Orders.Add(cancelled);

            db.SaveChanges();
        }
    }
}
