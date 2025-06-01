using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Domain.Enums
{
    /// <summary>
    /// Represents the various statuses an order can have during its lifecycle.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// The order has been created but not yet confirmed.
        /// </summary>
        Pending,

        /// <summary>
        /// The order has been confirmed and is being prepared for shipment.
        /// </summary>
        Confirmed,

        /// <summary>
        /// The order has been shipped to the customer.
        /// </summary>
        Shipped,

        /// <summary>
        /// The order has been delivered to the customer.
        /// </summary>
        Delivered,

        /// <summary>
        /// The order has been cancelled and will not be fulfilled.
        /// </summary>
        Cancelled
    }
}
