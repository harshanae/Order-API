using OrderApi.Application;
using OrderApi.Application.Repositories;
using OrderApi.Domain.Models;
using OrderApi.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Services
{
    public class OrderService
    {
        private OrderDbContext _context;
        private UnitOfWork _unitOfWork;
        private OrderRepository _orderRepo;
        public OrderService(OrderDbContext context)
        {
            this._context = context;
            this._unitOfWork = new UnitOfWork(context);
            _orderRepo = new OrderRepository(context);
        }

        public IEnumerable<Order> orderBulkAdd(IEnumerable<Order> orders)
        {
            List<Order> insertedOrders = new List<Order>();
            foreach (var o in orders)
            {
                _unitOfWork.OrderRepository.Insert(o);
                _unitOfWork.Save();

                var t = o;
                t.Id = o.Id;
                insertedOrders.Add(t);
            }

            return insertedOrders;
        }

        public decimal GetTotalSalesByCustomerId(int id)
        {
            decimal totalSale = 0;
            var orders = _unitOfWork.OrderRepository.GetAll();

            foreach(var order in orders)
            {
                if(order.CustomerId == id)
                {
                    totalSale += order.Total;
                }
            }

            return totalSale;
            
        }

        public decimal GetTotalSalesInDateRange(string startDate, string endDate)
        {
            decimal totalSales = 0;

            var ordersInRange = _unitOfWork.OrderRepository.GetAll().Where(order => order.CreatedDate > DateTime.Parse(startDate) && order.CreatedDate < DateTime.Parse(endDate));

            foreach(var order in ordersInRange)
            {
                totalSales += order.Total;
            }

            return totalSales;
        }

        public decimal GetTotalSalesByEmplyeeId(int id)
        {
            decimal totalSales = 0;
            var orders = _unitOfWork.OrderRepository.GetAll().Where(order => order.EmployeeId == id);

            foreach(var order in orders)
            {
                totalSales += order.Total;
            }

            return totalSales;
        }

        public IEnumerable<CustomerSalesDto> GetTotalSalesByCustomersGroupBy()
        {
            List<CustomerSalesDto> list = new List<CustomerSalesDto>();
            var orders = _unitOfWork.OrderRepository.GetAll().GroupBy(order => order.CustomerId).Select(g => new { 
                customerID = g.Key,
                total = g.Sum(order => order.Total)
            });

            foreach(var order in orders)
            {
                CustomerSalesDto temp = new CustomerSalesDto();
                var customer = _unitOfWork.CustomerRepository.GetById(order.customerID);
                temp.Customer = customer;
                temp.TotalSaleAmount = order.total;

                list.Add(temp);
            }

            return list;



        }

        public IEnumerable<CustomerTotal> GetCustomerSalesTotalGroupByRaw()
        {
            return _orderRepo.GetTotalSalesbyCustomerGroupByRep();
        }

        public IEnumerable<EmployeeTotal> GetEmployeeSalesGroupByRaw()
        {
            return _orderRepo.GetTotalSalesbyEmployeeGroupByRaw();
        }

        public IEnumerable<ProductOrderSummary> GetProductOrderSummary()
        {
            return _orderRepo.GetProductOrderSummaries();
        }

        public IEnumerable<EmployeeTotal> GetSalesByEmployeeSP()
        {
            return _orderRepo.GetTotalSalesByEmployeeSP();
        }

        
    }
}
