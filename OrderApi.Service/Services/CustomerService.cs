using OrderApi.Application;
using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Services
{
    public class CustomerService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly OrderDbContext _context;
        
        public CustomerService(OrderDbContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _unitOfWork.CustomerRepository.GetAll();
        }

        public Customer GetCustomerById(int id)
        {
            var customer = _unitOfWork.CustomerRepository.GetById(id);
            return customer;
        }

        public void AddCustomer(Customer customer)
        {
            _unitOfWork.CustomerRepository.Insert(customer);
            _unitOfWork.Save();
        }

        public void UpdateCustomer(int id, Customer updatedCustomer)
        {
            _unitOfWork.CustomerRepository.Update(updatedCustomer);
            _unitOfWork.Save();
        }
        
        public Customer GetCustomerByName(string name)
        {
            var allCustomers = this.GetAllCustomers();

            var customerFound = allCustomers.FirstOrDefault(cust => cust.CustomerName.Contains("name"));
            return customerFound;
        }

        public IEnumerable<Customer> customerBulkAdd(IEnumerable<Customer> customers)
        {
            List<Customer> insertedCustomers = new List<Customer>();
            foreach (var c in customers)
            {
                _unitOfWork.CustomerRepository.Insert(c);
                _unitOfWork.Save();

                var t = c;
                t.Id = c.Id;
                insertedCustomers.Add(t);
            }

            return insertedCustomers;
        }

        

        
    }
}
