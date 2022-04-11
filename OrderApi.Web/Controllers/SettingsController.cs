using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderApi.Application;
using OrderApi.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController: ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly SettingsService settingsService;

        public SettingsController(OrderDbContext context, ILoggerFactory logger)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            settingsService = new SettingsService(context);
            _logger = logger.CreateLogger("SettingsController");
        }

        [HttpGet]
        public async Task<ActionResult<int>> CleanDbofTrainingData()
        {
            _logger.LogInformation("Clean Training data was called");
            try
            {
                _unitOfWork.CleanTestData();
                return Ok(0);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return StatusCode(500);
            }
        } 


        
    }
}
