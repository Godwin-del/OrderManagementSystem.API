using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.DTOs
{
    /// <summary>
    /// Represents the data returned for a submitted or retrieved order.
    /// </summary>
    public class OrderResponseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the order.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the customer who placed the order.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the list of items included in the order.
        /// </summary>
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
