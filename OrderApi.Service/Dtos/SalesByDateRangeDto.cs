using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Dtos
{
    public class SalesByDateRangeDto
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public decimal TotalSales { get; set; }
    }
}
