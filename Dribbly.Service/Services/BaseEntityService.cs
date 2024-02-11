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
        IAuthContext _context;
        public BaseEntityService(IDbSet<T> dbSet,
        IAuthContext context) : base(dbSet)
        {
            _dbSet = dbSet;
            _context = context;
        }

        protected new void Add(T entity)
        {
            entity.DateAdded = DateTime.UtcNow;
            _dbSet.Add(entity);
        }

        protected bool Exists(long id)
        {
            return _dbSet.Any(e => e.Id == id);
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