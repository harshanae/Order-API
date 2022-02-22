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
    public class CustomersController : ControllerBase
    {
        private UnitOfWork unitOfWork;
        private readonly OrderDbContext _context;
        private readonly CustomerService customerService;
        private readonly OrderService orderService;
        private readonly ILogger _logger;

        public CustomersController(OrderDbContext context, ILoggerFactory logger)
        {
            _context = context;
            unitOfWork = new UnitOfWork(context);
            customerService = new CustomerService(context);
            orderService = new OrderService(context);
            _logger = logger.CreateLogger("CustomerController");
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            _logger.LogInformation("Get all customers was called");
            try
            {
                var customers = unitOfWork.CustomerRepository.GetAll();
                return Ok(customers);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            _logger.LogInformation("Customers get by id is called");
            try
            {
                var customer = unitOfWork.CustomerRepository.GetById(id);
                if (customer == null)
                {
                    return NotFound();
                }

                return customer;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            _logger.LogInformation("Update customer was called");
            if (id != customer.Id)
            {
                return BadRequest();
            }
            try
            {
                unitOfWork.CustomerRepository.Update(customer);
                unitOfWork.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _logger.LogInformation("Add customer was called");
            try
            {
                unitOfWork.CustomerRepository.Insert(customer);
                unitOfWork.Save();

                return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            _logger.LogInformation("Delete customer was called");
            try
            {
                var customer = unitOfWork.CustomerRepository.GetById(id);
                if (customer == null)
                {
                    return NotFound();
                }

                unitOfWork.CustomerRepository.Delete(id);
                unitOfWork.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkCustomers(IEnumerable<Customer> customers)
        {
            _logger.LogInformation("Customer bulk add was called"); try
            {
                if (customers.Count() == 0)
                {
                    return BadRequest();
                }
                return Ok(customerService.customerBulkAdd(customers));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        [HttpGet("GetSalesByCustomerId")]
        public async Task<ActionResult<IEnumerable<CustomerSalesDto>>> GetOrderByCustomerNo()
        {
            _logger.LogInformation("Get Sales by customer id was called");
            try
            {
                var id = Int32.Parse(HttpContext.Request.Query["customerId"].ToString());
                CustomerSalesDto cDto = new CustomerSalesDto();
                if (!CustomerExists(id))
                {
                    return BadRequest();
                }
                cDto.Customer = unitOfWork.CustomerRepository.GetById(id);
                cDto.TotalSaleAmount = orderService.GetTotalSalesByEmplyeeId(id);
                return Ok(cDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }


    }
}
