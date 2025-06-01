using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Application.DTOs
{
    /// <summary>
    /// Represents the data required to create a new order.
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// The identifier of the customer placing the order.
        /// </summary>
        [Required]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The list of items included in the order. Must contain at least one item.
        /// </summary>
        [Required]
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
