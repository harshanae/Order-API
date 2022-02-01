using OrderApi.Application;
using OrderApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Service.Services
{
    public class ProductService
    {
        private OrderDbContext _context;
        private UnitOfWork _unitOfWork;
        public ProductService(OrderDbContext context)
        {
            this._context = context;
            this._unitOfWork = new UnitOfWork(context);
        }

        public IEnumerable<Product> productBulkAdd(IEnumerable<Product> products)
        {
            List<Product> insertedProducts = new List<Product>();
            foreach (var p in products)
            {
                _unitOfWork.ProductRepository.Insert(p);
                _unitOfWork.Save();

                var t = p;
                t.Id = p.Id;
                insertedProducts.Append(t);
            }

            return insertedProducts;
        }

        public void DecreaseInventoryForProduct(int id, int quantity)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);
            product.Items-=quantity;

            _unitOfWork.ProductRepository.Update(product);

            try
            {
                _unitOfWork.Save();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }

        }
    }
}
