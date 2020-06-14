using System.Data.Entity;

namespace Dribbly.Service.Repositories
{
    public class BaseRepository<T> where T:class
    {
        public DbSet<T> _dbSet;
        protected BaseRepository(DbSet<T> dbSet)
        {
            _dbSet = dbSet;
        }

        public void Add(T model)
        {
            _dbSet.Add(model);
        }
    }
}