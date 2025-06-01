using OrderManagementSystem.Application.DTOs;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Application.Models;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IDiscountService _discountService;

        public OrderService(IOrderRepository repo, IDiscountService discountService)
        {
            _repo = repo;
            _discountService = discountService;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.Id = Guid.NewGuid();
            order.CreatedAt = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            await _repo.AddAsync(order);
            await _repo.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync() =>
            await _repo.GetAllAsync();

        public async Task<Order?> GetOrderByIdAsync(Guid id) =>
            await _repo.GetByIdAsync(id);

        public async Task<DiscountResult> GetDiscountForOrderAsync(Guid orderId)
        {
            var order = await _repo.GetByIdAsync(orderId)
                      ?? throw new KeyNotFoundException($"Order {orderId} not found");

            return _discountService.CalculateOrderDiscount(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null) return false;

            if (!IsValidTransition(order.Status, newStatus))
                return false;

            order.Status = newStatus;
            if (newStatus == OrderStatus.Delivered)
                order.FulfilledAt = DateTime.UtcNow;

            await _repo.UpdateAsync(order);
            await _repo.SaveChangesAsync();
            return true;
        }

        private bool IsValidTransition(OrderStatus current, OrderStatus next)
        {
            return current switch
            {
                OrderStatus.Pending => next == OrderStatus.Confirmed || next == OrderStatus.Cancelled,
                OrderStatus.Confirmed => next == OrderStatus.Shipped || next == OrderStatus.Cancelled,
                OrderStatus.Shipped => next == OrderStatus.Delivered,
                OrderStatus.Delivered => false,
                OrderStatus.Cancelled => false,
                _ => false
            };
        }

        public async Task<OrderAnalyticsDto> GetOrderAnalyticsAsync()
        {
            var allOrders = await _repo.GetAllAsync();
            var deliveredOrders = allOrders.Where(o => o.Status == OrderStatus.Delivered).ToList();

            if (!deliveredOrders.Any())
            {
                return new OrderAnalyticsDto
                {
                    AverageOrderValue = 0m,
                    AverageFulfillmentTimeHours = 0m
                };
            }

            var avgValue = deliveredOrders.Average(o => o.TotalAmount);
            var fulfilledOrders = deliveredOrders.Where(o => o.FulfilledAt.HasValue).ToList();

            decimal avgFulfillmentHours = fulfilledOrders.Any()
                ? (decimal)fulfilledOrders.Average(o => (o.FulfilledAt!.Value - o.CreatedAt).TotalHours)
                : 0m;

            return new OrderAnalyticsDto
            {
                AverageOrderValue = Math.Round((decimal)avgValue, 2),
                AverageFulfillmentTimeHours = Math.Round(avgFulfillmentHours, 2)
            };
        }
    }
}
