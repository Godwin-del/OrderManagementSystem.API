using OrderManagementSystem.Application.DTOs;
using OrderManagementSystem.Application.Models;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<DiscountResult> GetDiscountForOrderAsync(Guid orderId);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<OrderAnalyticsDto> GetOrderAnalyticsAsync();
    }
}
