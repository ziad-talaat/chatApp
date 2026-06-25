using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DataContext;
using Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Infrastructure.Repository
{
    internal class Repository<T>(AppDbContext context) : IRepository<T> where T : class
    {
        protected AppDbContext _context=context;
        public IQueryable<T> GetQuery { get => _context.Set<T>().AsQueryable(); }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<List<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T?> GetById(Guid id, bool track)
        {
            var query = _context.Set<T>().AsQueryable();

            if (!track)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(x => EF.Property<Guid>(x, "Id") == id);
        }
        public async Task<T?> GetById<P>(P id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Insert(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }
    }
}
