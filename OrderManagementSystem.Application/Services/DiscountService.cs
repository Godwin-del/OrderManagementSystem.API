using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Application.Models;
using OrderManagementSystem.Application.Rules;
using OrderManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly List<PromotionRule> _rules;

        public DiscountService(IEnumerable<PromotionRule> rules)
        {
            _rules = rules.ToList();
        }

        public DiscountResult CalculateOrderDiscount(Order order)
        {
            var result = new DiscountResult
            {
                OriginalTotal = order.TotalAmount
            };

            foreach (var rule in _rules)
            {
                var discount = rule.CalculateDiscount(order);
                if (discount > 0)
                {
                    result.DiscountAmount += discount;
                    result.AppliedRules.Add(rule.RuleName);
                }
            }

            if (result.DiscountAmount > result.OriginalTotal)
                result.DiscountAmount = result.OriginalTotal;

            return result;
        }
    }
}
