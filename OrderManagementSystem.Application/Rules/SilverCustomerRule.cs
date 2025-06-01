using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Application.Rules
{
    public class SilverCustomerRule : PromotionRule
    {
        public SilverCustomerRule() => RuleName = "Silver Customer 5% Off";

        public override decimal CalculateDiscount(Order order) =>
            order.Customer.Segment == CustomerSegment.Silver.ToString()
                ? order.TotalAmount * 0.05m
                : 0m;
    }
}
