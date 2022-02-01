using OrderApi.Application;
using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Services
{
    public class ShippingMethodService
    {
        private OrderDbContext _context;
        private UnitOfWork _unitOfWork;
        public ShippingMethodService(OrderDbContext context)
        {
            this._context = context;
            this._unitOfWork = new UnitOfWork(context);
        }

        public IEnumerable<ShippingMethod> shippingMethodBulkAdd(IEnumerable<ShippingMethod> shippingMethods)
        {
            List<ShippingMethod> insertedSMethods = new List<ShippingMethod>();
            foreach (var s in shippingMethods)
            {
                _unitOfWork.ShippingMethodsRepository.Insert(s);
                _unitOfWork.Save();

                var t = s;
                t.Id = s.Id;
                insertedSMethods.Append(t);
            }

            return insertedSMethods;
        }
    }
}
