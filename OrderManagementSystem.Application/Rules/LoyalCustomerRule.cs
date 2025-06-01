using OrderManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Rules
{
    public class LoyalCustomerRule : PromotionRule
    {
        public LoyalCustomerRule() => RuleName = "Loyal Customer 5% Off";

        public override decimal CalculateDiscount(Order order)
        {
            var oneYearAgo = DateTime.UtcNow.AddYears(-1);
            var pastYearOrders = order.Customer.Orders
                .Count(o => o.CreatedAt >= oneYearAgo && o.Id != order.Id);

            return pastYearOrders >= 5 ? order.TotalAmount * 0.05m : 0m;
        }
    }
}
