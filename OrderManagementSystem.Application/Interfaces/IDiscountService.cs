using OrderManagementSystem.Application.Models;
using OrderManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Interfaces
{
    public interface IDiscountService
    {
        DiscountResult CalculateOrderDiscount(Order order);
    }
}
