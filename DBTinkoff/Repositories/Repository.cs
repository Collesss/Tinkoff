using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTinkoff.Repositories
{
    public class Repository<Context, TEntity> : IRepository<TEntity> 
        where TEntity : class 
        where Context : DbContext
    {
        private Context _managerDbContext;
        public Repository(Context managerDbContext)
        {
            _managerDbContext = managerDbContext;
        }

        IQueryable<TEntity> IRepository<TEntity>.GetAll() =>
            _managerDbContext.Set<TEntity>().AsQueryable();

        async Task<TEntity> IRepository<TEntity>.CreateAsync(TEntity entity)
        {
            TEntity returnEntity = (await _managerDbContext.Set<TEntity>().AddAsync(entity)).Entity;
            await _managerDbContext.SaveChangesAsync();
            return returnEntity;
        }
        async Task IRepository<TEntity>.CreateAsync(IEnumerable<TEntity> entity)
        {
            await _managerDbContext.Set<TEntity>().AddRangeAsync(entity);
            await _managerDbContext.SaveChangesAsync();
        }

        async Task<TEntity> IRepository<TEntity>.DeleteAsync(TEntity entity)
        {
            TEntity returnEntity = (await Task.Run(() => _managerDbContext.Set<TEntity>().Remove(entity))).Entity;
            await _managerDbContext.SaveChangesAsync();
            return returnEntity;
        }
        async Task IRepository<TEntity>.DeleteAsync(IEnumerable<TEntity> entity)
        {
            await Task.Run(() => _managerDbContext.Set<TEntity>().RemoveRange(entity));
            await _managerDbContext.SaveChangesAsync();
        }

        async Task<TEntity> IRepository<TEntity>.UpdateAsync(TEntity entity)
        {
            TEntity returnEntity = (await Task.Run(() => _managerDbContext.Set<TEntity>().Update(entity))).Entity;
            await _managerDbContext.SaveChangesAsync();
            return returnEntity;
        }

        async Task IRepository<TEntity>.UpdateAsync(IEnumerable<TEntity> entity)
        {
            await Task.Run(() => _managerDbContext.Set<TEntity>().UpdateRange(entity));
            await _managerDbContext.SaveChangesAsync();
        }
    }
}
