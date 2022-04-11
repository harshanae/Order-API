using OrderApi.Domain.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application
{
    public interface IRepository<T> : IDisposable where T: BaseEntity
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Insert(T entity);
        void Delete(int id);
        void Update(T entity);

    }
}
