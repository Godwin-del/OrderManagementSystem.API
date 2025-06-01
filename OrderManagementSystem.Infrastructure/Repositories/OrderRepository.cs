using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Infrastructure.Data;
using OrderManagementSystem.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagementSystem.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderManagementSystemContext _db;

        public OrderRepository(OrderManagementSystemContext db) => _db = db;

        public async Task AddAsync(Order order)
        {
            await _db.Orders.AddAsync(order);
        }

        public async Task DeleteAsync(Guid orderId)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
                _db.Orders.Remove(order);
        }

        /// <summary>
        /// Returns all orders, including Items and Customer, in a read‐only, no‐tracking query.
        /// </summary>
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _db.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .ToListAsync();
        }

        /// <summary>
        /// Optionally return a paged subset of orders.
        /// </summary>
        public async Task<IEnumerable<Order>> GetPagedAsync(int page, int pageSize)
        {
            return await _db.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Finds a single order by ID, including Items and Customer.
        /// </summary>
        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _db.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public Task UpdateAsync(Order order)
        {
            _db.Orders.Update(order);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
