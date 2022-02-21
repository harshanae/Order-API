using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Domain.Models
{
    public class CustomerTotal
    {
        public int CustomerId { get; set; }
        public decimal Total { get; set; }

        public string CustomerName { get; set; }

        public string PhoneNumber { get; set; }
    }
}
