using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Repositories
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(OrderDbContext context) : base(context)
        {
        }

        public IEnumerable<CustomerTotal> GetTotalSalesbyCustomerGroupByRep()
        {
            var r = context.CustomerTotal.FromSqlRaw("SELECT o.customerId, c.customerName, c.phoneNumber, SUM(o.total) AS total FROM dbo.Orders o INNER JOIN  dbo.Customers c ON c.id=o.customerId GROUP BY o.customerId, c.customerName, c.phoneNumber").ToList();

            return r;
        }

        public IEnumerable<EmployeeTotal> GetTotalSalesbyEmployeeGroupByRaw()
        {
            var result = context.EmployeeTotal.FromSqlRaw("SELECT o.employeeId, e.firstName, e.phoneNumber, SUM(o.total) AS total FROM dbo.Orders o INNER JOIN  dbo.Employees e ON e.id = o.employeeId GROUP BY o.employeeId, e.firstName, e.phoneNumber").ToList();

            return result;
        }

        public IEnumerable<ProductOrderSummary> GetProductOrderSummaries()
        {
            var result = context.ProductOrderSummaries.FromSqlRaw("SELECT po.ProductId, p.ProductName, o.Total, p.Price, po.OrderId, po.Quantity FROM dbo.OrderDetails po INNER JOIN dbo.Orders o ON o.Id = po.OrderId INNER JOIN dbo.Products p ON p.Id=po.ProductId ").ToList();
            return result;
        }

        public IEnumerable<EmployeeTotal> GetTotalSalesByEmployeeSP()
        {
            var results = context.EmployeeTotal.FromSqlRaw("EXECUTE GetSalesByEmployeesSP");
            return results;
        }
    }
}
