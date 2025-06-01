using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Segment { get; set; } = default!;
        public DateTime DateJoined { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
