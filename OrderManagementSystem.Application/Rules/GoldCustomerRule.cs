using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Rules
{
    public class GoldCustomerRule : PromotionRule
    {
        public GoldCustomerRule() => RuleName = "Gold Customer 10% Off";

        public override decimal CalculateDiscount(Order order) =>
            order.Customer.Segment == CustomerSegment.Gold.ToString()
                ? order.TotalAmount * 0.10m
                : 0m;
    }
}
