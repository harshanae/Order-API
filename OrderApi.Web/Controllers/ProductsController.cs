﻿using System;
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
    public class ProductsController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService productService;
        private readonly OrderDetailsService orderDetailsService;
        private readonly ILogger _logger;

        public ProductsController(OrderDbContext context, ILoggerFactory logger)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            productService = new ProductService(context);
            orderDetailsService = new OrderDetailsService(context);
            _logger = logger.CreateLogger("ProductsController");
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            _logger.LogInformation("Get All products was called");
            try
            {
                return Ok(_unitOfWork.ProductRepository.GetAll());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            _logger.LogInformation("Get product by id was called");
            try
            {
                var product = _unitOfWork.ProductRepository.GetById(id);

                if (product == null)
                {
                    return NotFound();
                }

                return product;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            _logger.LogInformation("Update product was called");
            try
            {
                if (id != product.Id)
                {
                    return BadRequest();
                }

                _unitOfWork.ProductRepository.Update(product);

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
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _logger.LogInformation("Add product was called");
            try
            {
                _unitOfWork.ProductRepository.Insert(product);
                _unitOfWork.Save();

                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Delete product was called");
            try
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
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }

        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkProducts(IEnumerable<Product> products)
        {
            _logger.LogInformation("Product nulk add was called");
            try
            {
                if (products.Count() == 0)
                {
                    return BadRequest();
                }
                return Ok(productService.productBulkAdd(products));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        [HttpGet("GetSalesByProductId")]
        public async Task<ActionResult<IEnumerable<SalesByProductDto>>> GetSalesByProductNo()
        {
            _logger.LogInformation("Get sales by product was called");
            try
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
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }
    }
}
