using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Rules
{
    public class BronzeCustomerRule : PromotionRule
    {
        public BronzeCustomerRule() => RuleName = "Bronze Customer 2% Off";

        public override decimal CalculateDiscount(Order order) =>
            order.Customer.Segment == CustomerSegment.Bronze.ToString()
                ? order.TotalAmount * 0.02m
                : 0m;
    }
}
