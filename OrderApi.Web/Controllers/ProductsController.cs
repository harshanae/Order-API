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
    public class ProductsController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService productService;
        private readonly OrderDetailsService orderDetailsService;

        public ProductsController(OrderDbContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            productService = new ProductService(context);
            orderDetailsService = new OrderDetailsService(context);
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return Ok(_unitOfWork.ProductRepository.GetAll());
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _unitOfWork.ProductRepository.Update(product);

            try
            {
                _unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _unitOfWork.ProductRepository.Insert(product);
            _unitOfWork.Save();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            _unitOfWork.ProductRepository.Delete(id);
            _unitOfWork.Save();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkProducts(IEnumerable<Product> products)
        {
            if(products.Count() == 0)
            {
                return BadRequest();
            }
            return Ok(productService.productBulkAdd(products));
        }

        [HttpGet("GetSalesByProductId")]
        public async Task<ActionResult<IEnumerable<SalesByProductDto>>> GetSalesByProductNo()
        {
            var id = Int32.Parse(HttpContext.Request.Query["productId"].ToString());
            if (!ProductExists(id))
            {
                return BadRequest();
            }

            var resultDto = orderDetailsService.GetTotalSalesByProduct(id);

            decimal totalSales = 0;
            Product product = _unitOfWork.ProductRepository.GetById(id);
            totalSales = resultDto.Quantity * (product.Price - product.Price * (product.Discount / 100));

           // resultDto.Product = product;
            resultDto.TotalSales = totalSales;


            return Ok(resultDto);
        }
    }
}
