using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using Dribbly.Core.Models;

namespace Dribbly.Service.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntityModel
    {
        public DbSet<T> _dbSet;
        public BaseRepository(DbSet<T> dbSet)
        {
            _dbSet = dbSet;
        }

        public void Add(T model)
        {
            model.DateAdded = DateTime.UtcNow;
            _dbSet.Add(model);
        }

        public virtual IQueryable<T> Get(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = "",
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? take = null, int? skip = 0)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip > 0)
            {
                query = query.Skip(skip.Value);
            }

            if (take > 0)
            {
                query = query.Take(take.Value);
            }

            return query;
        }
    }

    public interface IBaseRepository<T>
    {
        void Add(T model);
        IQueryable<T> Get(
                Expression<Func<T, bool>> filter = null,
                string includeProperties = "",
                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                int? take = null, int? skip = 0);
    }

}