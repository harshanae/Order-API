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
    public class Order_ProductController : ControllerBase
    {
        private UnitOfWork _unitOfWork;
        private readonly OrderDbContext _context;
        private readonly OrderDetailsService orderDetailsService;

        public Order_ProductController(OrderDbContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            orderDetailsService = new OrderDetailsService(context);
        }

        // GET: api/Order_Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order_Product>>> GetOrderDetails()
        {
            return Ok(_unitOfWork.OrderDetailsRepository.GetAll());
        }

        // GET: api/Order_Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order_Product>> GetOrder_Product(int id)
        {
            var order_Product = _unitOfWork.OrderDetailsRepository.GetById(id);

            if (order_Product == null)
            {
                return NotFound();
            }

            return order_Product;
        }

        // PUT: api/Order_Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder_Product(int id, Order_Product order_Product)
        {
            if (id != order_Product.OrderId)
            {
                return BadRequest();
            }

            _unitOfWork.OrderDetailsRepository.Update(order_Product);

            try
            {
                _unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Order_ProductExists(order_Product.OrderId, order_Product.ProductId))
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

        // POST: api/Order_Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order_Product>> PostOrder_Product(Order_Product order_Product)
        {
            _unitOfWork.OrderDetailsRepository.Insert(order_Product);
            try
            {
                _unitOfWork.Save();
            }
            catch (DbUpdateException)
            {
                if (Order_ProductExists(order_Product.OrderId, order_Product.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrder_Product", new { id = order_Product.OrderId }, order_Product);
        }

        // DELETE: api/Order_Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder_Product(int id)
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

        private bool Order_ProductExists(int orderId, int productId)
        {
            return _context.OrderDetails.Any(e => e.OrderId == orderId && e.ProductId == productId);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Order_Product>>> PostBulkOrderDetails(IEnumerable<Order_Product> order_Products)
        {
            if (order_Products.Count() == 0)
            {
                return BadRequest();
            }
            return Ok(orderDetailsService.orderProductBulkAdd(order_Products));
        }
    }
}
