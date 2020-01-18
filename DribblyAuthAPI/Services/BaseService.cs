using DribblyAuthAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DribblyAuthAPI.Services
{
    public class BaseService<T> where T : BaseModel
    {
        DbSet<T> _dbSet;
        public BaseService(DbSet<T> dbSet)
        {
            _dbSet = dbSet;
        }

        protected IEnumerable<T> All()
        {
            return _dbSet;
        }

        protected void Add(T entity)
        {
            entity.DateAdded = DateTime.Now;
            _dbSet.Add(entity);
        }
    }
}