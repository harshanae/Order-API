using OrderApi.Application;
using OrderApi.Domain.Models;
using OrderApi.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Services
{
    public class OrderDetailsService
    {
        private readonly OrderDbContext _context;
        private readonly UnitOfWork _unitOfWork;

        public OrderDetailsService(OrderDbContext context)
        {
            this._context = context;
            this._unitOfWork = new UnitOfWork(context);
        }

        public CustomerSalesDto getTotalSalesForCustomer(int id)
        {
            var allOrders = _unitOfWork.OrderRepository.GetAll();
            var customer = _unitOfWork.CustomerRepository.GetById(id);
            decimal totalSalesPrice = 0;
            CustomerSalesDto customerSalesDto = new CustomerSalesDto();

           foreach (var order in allOrders)
            {
                if(order.CustomerId == id)
                {
                    totalSalesPrice += order.Total;
                }
            }

            customerSalesDto.Customer = customer;
            customerSalesDto.TotalSaleAmount = totalSalesPrice;

            return customerSalesDto;
        }

        public IEnumerable<Order_Product> orderProductBulkAdd(IEnumerable<Order_Product> order_Products)
        {
            List<Order_Product> insertedOrderProducts = new List<Order_Product>();
            foreach (var op in order_Products)
            {
                _unitOfWork.OrderDetailsRepository.Insert(op);
                _unitOfWork.Save();

                var t = op;
                t.Id = op.Id;
                insertedOrderProducts.Add(t);
            }

            return insertedOrderProducts;
        }

        public SalesByProductDto GetTotalSalesByProduct(int id)
        {
            SalesByProductDto resultDto = new SalesByProductDto();
            Product product = _unitOfWork.ProductRepository.GetById(id);
            var allOrderDetails = _unitOfWork.OrderDetailsRepository.GetAll().Where(order => order.ProductId == id);

            int quantity = 0;
            decimal totalSales = 0;
             foreach(var orderD in allOrderDetails)
            {
                quantity += orderD.Quantity;
            }

           // totalSales = quantity * (product.Price - product.Price*(product.Discount/100));

            resultDto.Quantity = quantity;
            resultDto.ProductName = product.ProductName;
            resultDto.TotalSales = totalSales;
            return resultDto;
        }
    }
}
