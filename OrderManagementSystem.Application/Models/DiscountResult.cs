using System;
using System.Collections.Generic;

namespace OrderManagementSystem.Application.Models
{
    /// <summary>
    /// Represents the result of a discount calculation for an order.
    /// </summary>
    public class DiscountResult
    {
        /// <summary>
        /// The total amount of the order before any discounts are applied.
        /// </summary>
        public decimal OriginalTotal { get; set; }

        /// <summary>
        /// The total dollar value of all discounts applied to the order.
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// The final total after subtracting the <see cref="DiscountAmount"/> from the <see cref="OriginalTotal"/>.
        /// </summary>
        public decimal FinalTotal => OriginalTotal - DiscountAmount;

        /// <summary>
        /// A list of promotion rule names that were applied when calculating the discount.
        /// </summary>
        public List<string> AppliedRules { get; set; } = new();
    }
}
