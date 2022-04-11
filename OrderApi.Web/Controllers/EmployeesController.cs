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
            try
            {
                return Ok(_unitOfWork.EmployeeRepository.GetAll());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
           
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            _logger.LogInformation("Get Employee by id was called");
            try
            {
                var employee = _unitOfWork.EmployeeRepository.GetById(id);

                if (employee == null)
                {
                    return NotFound();
                }

                return employee;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            _logger.LogInformation("Update Employee was called");
            try
            {
                if (id != employee.EmployeeId)
                {
                    return BadRequest();
                }

                _unitOfWork.EmployeeRepository.Update(employee);
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
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _logger.LogInformation("Add Employee was called");
            try
            {
                _unitOfWork.EmployeeRepository.Insert(employee);
                _unitOfWork.Save();

                return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogInformation("Delete Employee was called");
            try
            {
                var employee = _unitOfWork.EmployeeRepository.GetById(id);
                if (employee == null)
                {
                    return NotFound();
                }

                _unitOfWork.EmployeeRepository.Delete(id);
                _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkEmployee(IEnumerable<Employee> employees)
        {
            _logger.LogInformation("Employee bulk add was called");
            try
            {
                if (employees.Count() == 0)
                {
                    return BadRequest();
                }
                return Ok(employeeService.employeeBulkAdd(employees));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }

        [HttpGet("GetSalesByEmployeeId")]
        public async Task<ActionResult<IEnumerable<EmployeeSalesDto>>> GetOrderByCustomerNo()
        {
            _logger.LogInformation("Get sales by Employee id was called");
            try
            {
                var id = Int32.Parse(HttpContext.Request.Query["employeeId"].ToString());
                EmployeeSalesDto eDto = new EmployeeSalesDto();
                if (!EmployeeExists(id))
                {
                    return BadRequest();
                }
                eDto.Employee = _unitOfWork.EmployeeRepository.GetById(id);
                eDto.TotalSaleAmount = orderService.GetTotalSalesByEmplyeeId(id);
                return Ok(eDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }

        [HttpGet("GetEmployeesByFristName")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByFirstNameSP()
        {
            try
            {
                var firstName = HttpContext.Request.Query["employeeFirstName"].ToString();
                _logger.LogWarning(firstName);
                return Ok(employeeService.GetEmployeeByFirstNameStoreProcedure(firstName));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }
    }
}
