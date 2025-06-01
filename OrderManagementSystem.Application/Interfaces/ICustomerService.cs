using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<bool> CustomerExistsAsync(Guid customerId);
    }
}
