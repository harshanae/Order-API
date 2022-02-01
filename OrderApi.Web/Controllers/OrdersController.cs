using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application;
using OrderApi.Domain.Models;
using OrderApi.Service.Dtos;
using OrderApi.Service.Services;

namespace OrderApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly OrderService orderService;

        public OrdersController(OrderDbContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            orderService = new OrderService(context);
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return Ok(_unitOfWork.OrderRepository.GetAll());
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = _unitOfWork.OrderRepository.GetById(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _unitOfWork.OrderRepository.Update(order);

            try
            {
                _unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _unitOfWork.OrderRepository.Insert(order);
            _unitOfWork.Save();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = _unitOfWork.OrderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            _unitOfWork.OrderRepository.Delete(id);
            _unitOfWork.Save();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkOrders(IEnumerable<Order> orders)
        {
            if (orders.Count() == 0)
            {
                return BadRequest();
            }
            return Ok(orderService.orderBulkAdd(orders));
        }

        [HttpPost("GetSalesByDateRange")]
        public async Task<ActionResult<IEnumerable<SalesByDateRangeDto>>> GetOrderByCustomerNo(SalesTotalDateRangeRequestDto requestDto)
        {
            if(requestDto.startDate == null || requestDto.endDate == null)
            {
                return BadRequest();
            }
            var startDate = requestDto.startDate;
            var endDate = requestDto.endDate;

            SalesByDateRangeDto resultDto = new SalesByDateRangeDto();
            resultDto.TotalSales = orderService.GetTotalSalesInDateRange(startDate, endDate);

            resultDto.StartDate = startDate;
            resultDto.EndDate = endDate;
            return Ok(resultDto);
        }

        [HttpGet("GetSalesByCustomerGroupBy")]
        public async Task<ActionResult<IEnumerable<CustomerSalesDto>>> GetSalesByCustomerGroupBy()
        {

            return Ok(orderService.GetTotalSalesByCustomersGroupBy());
        }
    }
}
