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
    public class EmployeesController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly EmployeeService employeeService;
        private readonly OrderService orderService;
        private readonly ILogger _logger;

        public EmployeesController(OrderDbContext context, ILoggerFactory logger)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            employeeService = new EmployeeService(context);
            orderService = new OrderService(context);
            _logger = logger.CreateLogger("EmployeeController");
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            _logger.LogInformation("Get Employees was called");
            return Ok(_unitOfWork.EmployeeRepository.GetAll());
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            _logger.LogInformation("Get Employee by id was called");
            var employee = _unitOfWork.EmployeeRepository.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            _logger.LogInformation("Update Employee was called");
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }

            _unitOfWork.EmployeeRepository.Update(employee);

            try
            {
                _unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _logger.LogInformation("Add Employee was called");
            _unitOfWork.EmployeeRepository.Insert(employee);
            _unitOfWork.Save();

            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogInformation("Delete Employee was called");
            var employee = _unitOfWork.EmployeeRepository.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }

            _unitOfWork.EmployeeRepository.Delete(id);
            _unitOfWork.Save();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkEmployee(IEnumerable<Employee> employees)
        {
            _logger.LogInformation("Employee bulk add was called");
            if (employees.Count() == 0)
            {
                return BadRequest();
            }
            return Ok(employeeService.employeeBulkAdd(employees));
        }

        [HttpGet("GetSalesByEmployeeId")]
        public async Task<ActionResult<IEnumerable<EmployeeSalesDto>>> GetOrderByCustomerNo()
        {
            _logger.LogInformation("Get sales by Employee id was called");
            var id = Int32.Parse(HttpContext.Request.Query["employeeId"].ToString());
            EmployeeSalesDto eDto = new EmployeeSalesDto() ;
            if(!EmployeeExists(id))
            {
                return BadRequest();
            }
            eDto.Employee = _unitOfWork.EmployeeRepository.GetById(id);
            eDto.TotalSaleAmount = orderService.GetTotalSalesByEmplyeeId(id);
            return Ok(eDto);
        }
    }
}
