using OrderApi.Application;
using OrderApi.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Services
{
    public class SettingsService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly OrderDbContext _context;
        private readonly SettingsRepository settingsRepostory;

        public SettingsService(OrderDbContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
            settingsRepostory = new SettingsRepository(context);
        }

        public void CleanTestData()
        {
            settingsRepostory.CleanTestData();
        }
    }
}
