using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application
{
    public class UnitOfWork : IDisposable
    {
        private OrderDbContext _context;
        private bool disposed = false;

        public UnitOfWork(OrderDbContext context)
        {
            _context = context;
        }

        private IRepository<Customer> customerRepository;
        private IRepository<Employee> employeeRepository;
        private IRepository<Product> productRepository;
        private IRepository<Order_Product> orderDetailsRepository;
        private IRepository<Order> orderRepository;
        private IRepository<ShippingMethod> shippingMethodsRepository;

        public IRepository<Customer> CustomerRepository
        {
            get
            {
                if(this.customerRepository == null)
                {
                    this.customerRepository = new Repository<Customer>(_context);
                }
                return this.customerRepository;
            }
        }

        public IRepository<Employee> EmployeeRepository
        {
            get
            {
                if (this.employeeRepository == null)
                {
                    this.employeeRepository = new Repository<Employee>(_context);
                }
                return this.employeeRepository;
            }
        }

        public IRepository<Product> ProductRepository
        {
            get
            {
                if (this.productRepository == null)
                {
                    this.productRepository = new Repository<Product>(_context);
                }
                return this.productRepository;
            }
        }

        public IRepository<Order_Product> OrderDetailsRepository
        {
            get
            {
                if (this.orderDetailsRepository == null)
                {
                    this.orderDetailsRepository = new Repository<Order_Product>(_context);
                }
                return this.orderDetailsRepository;
            }
        }

        public IRepository<Order> OrderRepository
        {
            get
            {
                if (this.orderRepository == null)
                {
                    this.orderRepository = new Repository<Order>(_context);
                }
                return this.orderRepository;
            }
        }

        public IRepository<ShippingMethod> ShippingMethodsRepository
        {
            get
            {
                if (this.shippingMethodsRepository == null)
                {
                    this.shippingMethodsRepository = new Repository<ShippingMethod>(_context);
                }
                return this.shippingMethodsRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    _context.Dispose();
                }
            }

            this.disposed = true;
        } 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            //throw new NotImplementedException();
        }
    }
}
