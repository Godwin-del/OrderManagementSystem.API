using OrderManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Rules
{
    public abstract class PromotionRule
    {
        public string RuleName { get; set; } = default!;
        /// <summary>
        /// Calculates the discount amount (absolute) to apply on the given order.
        /// </summary>
        public abstract decimal CalculateDiscount(Order order);
    }
}
