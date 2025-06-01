using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.DTOs
{
    /// <summary>
    /// Represents summarized analytics data for orders in the system.
    /// </summary>
    public class OrderAnalyticsDto
    {
        /// <summary>
        /// Gets or sets the average value of all orders.
        /// </summary>
        public decimal AverageOrderValue { get; set; }

        /// <summary>
        /// Gets or sets the average time (in hours) taken to fulfill orders.
        /// </summary>
        public decimal AverageFulfillmentTimeHours { get; set; }
    }
}
