using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTinkoff.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> CreateAsync(TEntity entity);
        Task CreateAsync(IEnumerable<TEntity> entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task UpdateAsync(IEnumerable<TEntity> entity);
        Task<TEntity> DeleteAsync(TEntity entity);
        Task DeleteAsync(IEnumerable<TEntity> entity);
    }
}
