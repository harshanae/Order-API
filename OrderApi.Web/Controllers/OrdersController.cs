using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        public OrdersController(OrderDbContext context, ILoggerFactory logger)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            orderService = new OrderService(context);
            _logger = logger.CreateLogger("OrdersController");
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            _logger.LogInformation("Get all Orders was called");
            try
            {
                return Ok(_unitOfWork.OrderRepository.GetAll());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            _logger.LogInformation("Get Order by id was called");
            try
            {
                var order = _unitOfWork.OrderRepository.GetById(id);

                if (order == null)
                {
                    return NotFound();
                }

                return order;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            _logger.LogInformation("Update Order was called");
            if (id != order.Id)
            {
                return BadRequest();
            }

            _unitOfWork.OrderRepository.Update(order);

            try
            {
                _unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(e, "Something went wrong");
                    return StatusCode(500);
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _logger.LogInformation("Add Order was called");
            try
            {
                _unitOfWork.OrderRepository.Insert(order);
                _unitOfWork.Save();

                return CreatedAtAction("GetOrder", new { id = order.Id }, order);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
           
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation("Delete Order was called");
            try
            {
                _unitOfWork.OrderRepository.Delete(id);
                _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            var order = _unitOfWork.OrderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkOrders(IEnumerable<Order> orders)
        {
            _logger.LogInformation("Order bulk add was called");
            if (orders.Count() == 0)
            {
                return BadRequest();
            }
            try
            {
                return Ok(orderService.orderBulkAdd(orders));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
           
        }

        [HttpPost("GetSalesByDateRange")]
        public async Task<ActionResult<IEnumerable<SalesByDateRangeDto>>> GetOrderByCustomerNo(SalesTotalDateRangeRequestDto requestDto)
        {
            _logger.LogInformation("Get order by date range was called");
            if (requestDto.startDate == null || requestDto.endDate == null)
            {
                return BadRequest();
            }
            var startDate = requestDto.startDate;
            var endDate = requestDto.endDate;

            SalesByDateRangeDto resultDto = new SalesByDateRangeDto();
            try
            {
                resultDto.TotalSales = orderService.GetTotalSalesInDateRange(startDate, endDate);

                resultDto.StartDate = startDate;
                resultDto.EndDate = endDate;
                return Ok(resultDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        [HttpGet("GetSalesByCustomerGroupBy")]
        public async Task<ActionResult<IEnumerable<CustomerTotal>>> GetSalesByCustomerGroupBy()
        {
            _logger.LogInformation("Get order by customer group by was called");
            try
            {
                //return Ok(orderService.GetTotalSalesByCustomersGroupBy());
                return Ok(orderService.GetCustomerSalesTotalGroupByRaw());
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        [HttpGet("GetSalesByEmployeesGroupBy")]
        public async Task<ActionResult<IEnumerable<EmployeeTotal>>> GetSalesByEmployeeGroupBy()
        {
            _logger.LogInformation("Get order by employee group by was called");
            try
            {
                return Ok(orderService.GetEmployeeSalesGroupByRaw());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }

        [HttpGet("GetProductOrderSummaries")]
        public async Task<ActionResult<IEnumerable<ProductOrderSummary>>> GetProductOrderSummary()
        {
            _logger.LogInformation("Get Product Order summary was called");
            try
            {
                return Ok(orderService.GetProductOrderSummary());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }
    }
}
