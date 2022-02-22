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
using OrderApi.Service.Services;

namespace OrderApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingMethodsController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly ShippingMethodService shippingMethodService;
        private readonly ILogger _logger;

        public ShippingMethodsController(OrderDbContext context, ILoggerFactory logger)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            shippingMethodService = new ShippingMethodService(context);
            _logger = logger.CreateLogger("Shipping Methods Controller");
        }

        // GET: api/ShippingMethods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingMethod>>> GetShippingMethods()
        {
            _logger.LogInformation("Get all shipping methods was called");
            try
            {
                return Ok(_unitOfWork.ShippingMethodsRepository.GetAll());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }

        // GET: api/ShippingMethods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingMethod>> GetShippingMethod(int id)
        {
            _logger.LogInformation("Get Shipping method by id was called");
            try
            {
                var shippingMethod = _unitOfWork.ShippingMethodsRepository.GetById(id);

                if (shippingMethod == null)
                {
                    return NotFound();
                }

                return shippingMethod;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }

        // PUT: api/ShippingMethods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShippingMethod(int id, ShippingMethod shippingMethod)
        {
            _logger.LogInformation("Update shipping method was called");
            try
            {
                if (id != shippingMethod.Id)
                {
                    return BadRequest();
                }

                _unitOfWork.ShippingMethodsRepository.Update(shippingMethod);

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
                if (!ShippingMethodExists(id))
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

        // POST: api/ShippingMethods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShippingMethod>> PostShippingMethod(ShippingMethod shippingMethod)
        {
            _logger.LogInformation("Add shipping method was called");
            try
            {
                _unitOfWork.ShippingMethodsRepository.Insert(shippingMethod);
                _unitOfWork.Save();
                return CreatedAtAction("GetShippingMethod", new { id = shippingMethod.Id }, shippingMethod);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }

        // DELETE: api/ShippingMethods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingMethod(int id)
        {
            _logger.LogInformation("Delete shipping method was called");
            try
            {
                var shippingMethod = _unitOfWork.ShippingMethodsRepository.GetById(id);
                if (shippingMethod == null)
                {
                    return NotFound();
                }
                _unitOfWork.ShippingMethodsRepository.Delete(id);
                _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        }

        private bool ShippingMethodExists(int id)
        {
            return _context.ShippingMethods.Any(e => e.Id == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkShippingMethods(IEnumerable<ShippingMethod> smethods)
        {
            _logger.LogInformation("Shipping method bulk add was called");
            try
            {
                if (smethods.Count() == 0)
                {
                    return BadRequest();
                }
                return Ok(shippingMethodService.shippingMethodBulkAdd(smethods));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
            
        }
    }
}
