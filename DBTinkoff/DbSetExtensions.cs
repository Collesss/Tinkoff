using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTinkoff
{
    public static class DbSetExtensions
    {
        public static async Task Merge<TEntity>(this DbSet<TEntity> dbSet, 
            IEnumerable<TEntity> entities, IEqualityComparer<TEntity> entityComparer, 
            IEqualityComparer<TEntity> entityKeyComparer) where TEntity : class
        {
            //var dbEntities = await dbSet.Where(entity => entities.Any(el => entityKeyComparer.Equals(entity, el))).AsNoTracking().ToListAsync();

            var create = entities.Except(dbSet, entityKeyComparer);
            var update = entities.Except(dbSet, entityComparer).Except(create, entityKeyComparer);

            await dbSet.AddRangeAsync(create);
            dbSet.UpdateRange(update);
        }
    }
}
