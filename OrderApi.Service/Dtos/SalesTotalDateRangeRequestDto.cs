using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Dtos
{
    public class SalesTotalDateRangeRequestDto
    {
        public string startDate { get; set; }
        public string endDate { get; set; }
    }
}
