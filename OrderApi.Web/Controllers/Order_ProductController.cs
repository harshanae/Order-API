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
    public class Order_ProductController : ControllerBase
    {
        private UnitOfWork _unitOfWork;
        private readonly OrderDbContext _context;
        private readonly OrderDetailsService orderDetailsService;
        private readonly ILogger _logger;

        public Order_ProductController(OrderDbContext context, ILoggerFactory logger)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            orderDetailsService = new OrderDetailsService(context);
            _logger = logger.CreateLogger("Order_ProductController");
        }

        // GET: api/Order_Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order_Product>>> GetOrderDetails()
        {
            _logger.LogInformation("Get all Sales Details was called");
            try
            {
                return Ok(_unitOfWork.OrderDetailsRepository.GetAll());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        // GET: api/Order_Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order_Product>> GetOrder_Product(int id)
        {
            _logger.LogInformation("Get all Sales Details by id was called");
            try
            {
                var order_Product = _unitOfWork.OrderDetailsRepository.GetById(id);

                if (order_Product == null)
                {
                    return NotFound();
                }

                return order_Product;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        // PUT: api/Order_Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder_Product(int id, Order_Product order_Product)
        {
            _logger.LogInformation("Update Sales Details was called");
            try
            {
                if (id != order_Product.OrderId)
                {
                    return BadRequest();
                }

                _unitOfWork.OrderDetailsRepository.Update(order_Product);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            

            try
            {
                _unitOfWork.Save();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!Order_ProductExists(order_Product.OrderId, order_Product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(e, "Something went wrong");
                    return StatusCode(500);
                }
            }
        }

        // POST: api/Order_Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order_Product>> PostOrder_Product(Order_Product order_Product)
        {
            _logger.LogInformation("Add Sales Details was called");
            try
            {
                _unitOfWork.OrderDetailsRepository.Insert(order_Product);
                _unitOfWork.Save();

                return CreatedAtAction("GetOrder_Product", new { id = order_Product.OrderId }, order_Product);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
            //try
            //{
                
            //}
            //catch (DbUpdateException)
            //{
            //    if (Order_ProductExists(order_Product.OrderId, order_Product.ProductId))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

        }

        // DELETE: api/Order_Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder_Product(int id)
        {
            _logger.LogInformation("Delete Sales Details was called");
            try
            {
                var order_Product = _unitOfWork.OrderDetailsRepository.GetById(id);
                if (order_Product == null)
                {
                    return NotFound();
                }

                _unitOfWork.OrderDetailsRepository.Delete(id);
                _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        private bool Order_ProductExists(int orderId, int productId)
        {
            return _context.OrderDetails.Any(e => e.OrderId == orderId && e.ProductId == productId);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Order_Product>>> PostBulkOrderDetails(IEnumerable<Order_Product> order_Products)
        {
            _logger.LogInformation("Sales Details bulk add was called");
            try
            {
                if (order_Products.Count() == 0)
                {
                    return BadRequest();
                }
                return Ok(orderDetailsService.orderProductBulkAdd(order_Products));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }
    }
}
