using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Dtos
{
    public class CustomerSalesDto
    {
        public Customer Customer { get; set; }
        public decimal TotalSaleAmount { get; set; }
    }
}
