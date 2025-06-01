using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.DTOs
{
    /// <summary>
    /// Represents an individual item in an order, including product details and pricing.
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// Gets or sets the name of the product being ordered.
        /// </summary>
        [Required]
        public string ProductName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the quantity of the product in the order.
        /// Must be at least 1.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price per unit of the product.
        /// Must be greater than 0.
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0.")]
        public decimal UnitPrice { get; set; }
    }
}
