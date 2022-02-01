﻿using System;
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
    public class CustomersController : ControllerBase
    {
        private UnitOfWork unitOfWork;
        private readonly OrderDbContext _context;
        private readonly CustomerService customerService;
        private readonly OrderService orderService;

        public CustomersController(OrderDbContext context)
        {
            _context = context;
            unitOfWork = new UnitOfWork(context);
            customerService = new CustomerService(context);
            orderService = new OrderService(context);
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = unitOfWork.CustomerRepository.GetAll();
            return Ok(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = unitOfWork.CustomerRepository.GetById(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            unitOfWork.CustomerRepository.Update(customer);

            try
            {
                unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            unitOfWork.CustomerRepository.Insert(customer);
            unitOfWork.Save();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
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

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkCustomers(IEnumerable<Customer> customers)
        {
            if (customers.Count() == 0)
            {
                return BadRequest();
            }
            return Ok(customerService.customerBulkAdd(customers));
        }

        [HttpGet("GetSalesByCustomerId")]
        public async Task<ActionResult<IEnumerable<CustomerSalesDto>>> GetOrderByCustomerNo()
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


    }
}
