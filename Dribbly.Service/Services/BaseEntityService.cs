using Dribbly.Core.Models;
using Dribbly.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class BaseEntityService<T> : BaseService<T> where T : BaseEntityModel
    {
        public BaseEntityService(DbSet<T> dbSet) : base(dbSet)
        {
            _dbSet = dbSet;
        }

        protected new void Add(T entity)
        {
            entity.DateAdded = DateTime.UtcNow;
            _dbSet.Add(entity);
        }

        protected T GetById(long id)
        {
            return _dbSet.SingleOrDefault(e => e.Id == id);
        }

        protected async Task<T> GetByIdAsync(long id)
        {
            return await _dbSet.SingleOrDefaultAsync(e => e.Id == id);
        }

        protected new T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.SingleOrDefault(predicate);
        }
    }
}