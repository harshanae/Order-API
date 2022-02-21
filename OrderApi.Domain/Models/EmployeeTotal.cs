using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Domain.Models
{
    public class EmployeeTotal
    {
        public int EmployeeId { get; set; }
        public decimal Total { get; set; }

        public string FirstName { get; set; }

        public string PhoneNumber { get; set; }
    }
}
