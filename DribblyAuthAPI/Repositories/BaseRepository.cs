using System.Data.Entity;

namespace DribblyAuthAPI.Repositories
{
    public class BaseRepository<T> where T:class
    {
        DbSet<T> _dbSet;
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