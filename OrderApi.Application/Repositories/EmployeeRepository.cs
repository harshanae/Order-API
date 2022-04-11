using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Repositories
{
    public class EmployeeRepository : Repository<Employee>
    {
        public EmployeeRepository(OrderDbContext context) : base(context)
        {
        }

        public IEnumerable<Employee> SPGetEmployeeByFirstName(string firstname)
        {
            var result = context.Employees.FromSqlRaw("GetEmployeesByFirstName @p0", firstname).ToList();
            return result;
        }
    }
}
