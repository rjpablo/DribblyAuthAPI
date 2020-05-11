﻿using DribblyAuthAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace DribblyAuthAPI.Services
{
    public class BaseService<T> where T : BaseModel
    {
        protected DbSet<T> _dbSet;
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
            _dbSet.Add(entity);
        }

        protected void Update(T entity)
        {
            _dbSet.AddOrUpdate(entity);
        }

        protected T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.SingleOrDefault(predicate);
        }
    }
}