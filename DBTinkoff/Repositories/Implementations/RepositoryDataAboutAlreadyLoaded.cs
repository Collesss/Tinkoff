using DBTinkoff.Repositories.Interfaces;
using DBTinkoffEntities.Entities;
using DBTinkoffEntities.EqualityComparers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace DBTinkoff.Repositories.Implementations
{
    public class RepositoryDataAboutAlreadyLoaded : IRepositoryDataAboutAlreadyLoaded
    {
        private readonly DBTinkoffContext _dBTinkoffContext;

        public RepositoryDataAboutAlreadyLoaded(DBTinkoffContext dBTinkoffContext)
        {
            _dBTinkoffContext = dBTinkoffContext;
        }

        async Task<IEnumerable<(DateTime from, DateTime to)>> IRepositoryDataAboutAlreadyLoaded.GetNotLoadIntervals(string figi, DateTime from, DateTime to, CandleInterval interval)
        {
            if (to > DateTime.UtcNow.Date.AddDays(1))
                to = DateTime.UtcNow.Date.AddDays(1);

            if (from >= to)
                return new List<(DateTime from, DateTime to)>();

            from = from.AddDays(-1);

            DateTime last = from;

            int group = 0;

            return (await _dBTinkoffContext.DataAboutLoadeds
                .Where(dAL => dAL.Figi == figi && dAL.Interval == interval && dAL.Time > from && dAL.Time < to)
                .Select(dAL => dAL.Time)
                .OrderBy(time => time)
                .Prepend(from)
                .Append(to)
                .ToListAsync())
                .GroupBy(time =>
                {
                    bool incGroup = time - last < TimeSpan.FromDays(1);
                    last = time;
                    return incGroup ? ++group : group;
                })
                .Where(gr => gr.Count() > 1)
                .Select(gr => (from: gr.Min().AddDays(1), to: gr.Max()))
                .ToList();
        }

        async Task IRepositoryDataAboutAlreadyLoaded.SetLoadInterval(string figi, DateTime from, DateTime to, CandleInterval interval)
        {
            if (to > DateTime.UtcNow.Date.AddDays(-1))
                to = DateTime.UtcNow.Date.AddDays(-1);

            if (from >= to)
                return;

            await _dBTinkoffContext.DataAboutLoadeds.Merge(Enumerable.Range(0, (to - from).Days)
                .Select(i => new EntityDataAboutAlreadyLoaded(figi, from.AddDays(i), interval)), 
                new EntityDataAboutAlreadyLoadedEqualityComparer(), new EntityDataAboutAlreadyLoadedEqualityComparer());

            await _dBTinkoffContext.SaveChangesAsync();
        }
    }
}
