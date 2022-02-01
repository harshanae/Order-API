using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Dtos
{
    public class SalesByProductDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public decimal TotalSales { get; set; }
    }
}
