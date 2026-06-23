using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IRepository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetQuery { get; }
        Task<List<T>> GetAll();
        Task<T?> GetById(Guid id);
        Task<T?> GetById(Guid id,bool track);

        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
