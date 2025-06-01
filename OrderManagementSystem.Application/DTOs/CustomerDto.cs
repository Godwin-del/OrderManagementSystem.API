using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace OrderManagementSystem.Application.DTOs
{
    /// <summary>
    /// Represents a customer with their segment, join date, and associated orders.
    /// </summary>
    public class CustomerDto
    {
        /// <summary>
        /// The unique identifier of the customer.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The loyalty segment of the customer (e.g., Bronze, Silver, Gold).
        /// </summary>
        public CustomerSegment Segment { get; set; }

        /// <summary>
        /// The date and time when the customer joined.
        /// </summary>
        public DateTime DateJoined { get; set; }

        /// <summary>
        /// The collection of orders placed by this customer.
        /// </summary>
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
