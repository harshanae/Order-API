using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Repositories
{
     public class SettingsRepository : Repository<Customer>
    {
        public SettingsRepository(OrderDbContext context) : base(context)
        {
        }

        public virtual void CleanTestData()
        {
            context.Database.ExecuteSqlRaw("dbo.CleanTestingDataFromDB");
        }
    }
}
