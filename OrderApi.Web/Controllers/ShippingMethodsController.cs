using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ShippingMethodsController(OrderDbContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            shippingMethodService = new ShippingMethodService(context);
        }

        // GET: api/ShippingMethods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingMethod>>> GetShippingMethods()
        {
            return Ok(_unitOfWork.ShippingMethodsRepository.GetAll());
        }

        // GET: api/ShippingMethods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingMethod>> GetShippingMethod(int id)
        {
            var shippingMethod = _unitOfWork.ShippingMethodsRepository.GetById(id);

            if (shippingMethod == null)
            {
                return NotFound();
            }

            return shippingMethod;
        }

        // PUT: api/ShippingMethods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShippingMethod(int id, ShippingMethod shippingMethod)
        {
            if (id != shippingMethod.Id)
            {
                return BadRequest();
            }

            _unitOfWork.ShippingMethodsRepository.Update(shippingMethod);

            try
            {
                _unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingMethodExists(id))
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

        // POST: api/ShippingMethods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShippingMethod>> PostShippingMethod(ShippingMethod shippingMethod)
        {
            _unitOfWork.ShippingMethodsRepository.Insert(shippingMethod);
            _unitOfWork.Save();
            return CreatedAtAction("GetShippingMethod", new { id = shippingMethod.Id }, shippingMethod);
        }

        // DELETE: api/ShippingMethods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingMethod(int id)
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

        private bool ShippingMethodExists(int id)
        {
            return _context.ShippingMethods.Any(e => e.Id == id);
        }

        [HttpPost("bulkAdd")]
        public async Task<ActionResult<IEnumerable<Employee>>> PostBulkShippingMethods(IEnumerable<ShippingMethod> smethods)
        {
            if (smethods.Count() == 0)
            {
                return BadRequest();
            }
            return Ok(shippingMethodService.shippingMethodBulkAdd(smethods));
        }
    }
}
