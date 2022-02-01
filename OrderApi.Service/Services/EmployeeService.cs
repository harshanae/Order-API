using OrderApi.Application;
using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Services
{
    public class EmployeeService
    {
        private OrderDbContext _context;
        private UnitOfWork _unitOfWork;
        public EmployeeService(OrderDbContext context)
        {
            this._context = context;
            this._unitOfWork = new UnitOfWork(context);
        }

        public IEnumerable<Employee> employeeBulkAdd(IEnumerable<Employee> employees)
        {
           List<Employee> insertedEmployees = new List<Employee>(); 
            foreach(var e in employees)
            {
                _unitOfWork.EmployeeRepository.Insert(e);
                _unitOfWork.Save();

                var t = e;
                t.EmployeeId = e.EmployeeId;
                insertedEmployees.Add(t);
            }

            return insertedEmployees;
        }
    }
}
