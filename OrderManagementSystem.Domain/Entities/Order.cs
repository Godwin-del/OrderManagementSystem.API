using OrderManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderManagementSystem.Domain.Entities
{
    /// <summary>
    /// Represents a customer order, including its items, status, and timestamps.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Unique identifier of the order.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key of the customer who placed the order.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Navigation property to the <see cref="Customer"/> who placed this order.
        /// </summary>
        public Customer Customer { get; set; } = default!;

        /// <summary>
        /// Date and time when the order was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the order was fulfilled (delivered). Null if not yet delivered.
        /// </summary>
        public DateTime? FulfilledAt { get; set; }

        /// <summary>
        /// Current status of the order.
        /// </summary>
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        /// <summary>
        /// Collection of line items included in this order.
        /// </summary>
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        /// <summary>
        /// Calculates the total amount for this order by summing (Quantity × UnitPrice) for each item.
        /// </summary>
        public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
