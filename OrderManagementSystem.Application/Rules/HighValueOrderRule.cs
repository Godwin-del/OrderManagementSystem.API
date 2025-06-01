using OrderManagementSystem.Domain.Entities;

namespace OrderManagementSystem.Application.Rules
{
    public class HighValueOrderRule : PromotionRule
    {
        public HighValueOrderRule() => RuleName = "High-Value Order $25 Off";

        public override decimal CalculateDiscount(Order order)
        {
            return order.TotalAmount >= 500m ? 25m : 0m;
        }
    }
}
