using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly OrderManagementSystemContext _context;

        public CustomerService(OrderManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<bool> CustomerExistsAsync(Guid customerId)
        {
            return await _context.Customers.AnyAsync(c => c.Id == customerId);
        }
    }
}
