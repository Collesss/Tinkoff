using AutoMapper;
using DBTinkoff.Repositories.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using Microsoft.EntityFrameworkCore;
using DBTinkoffEntities.EqualityComparers;
using DBTinkoffEntities.Entities;

namespace DBTinkoff.Repositories.Implementations
{
    public class RepositoryCandlePayload : IRepositoryCandlePayload
    {
        private readonly IConnection<IContext> _connection;
        private readonly DBTinkoffContext _dBTinkoffContext;
        private readonly IMapper _mapper;

        public RepositoryCandlePayload(IConnection<IContext> connection, DBTinkoffContext dBTinkoffContext, IMapper mapper)
        {
            _connection = connection;
            _dBTinkoffContext = dBTinkoffContext;
            _mapper = mapper;
        }
        async Task<IEnumerable<CandlePayload>> IRepositoryCandlePayload.MarketCandleAsync(string figi, DateTime from, DateTime to, CandleInterval interval)
        {
            /*
            if (to > DateTime.Today.AddDays(1))
                to = DateTime.Today.AddDays(1);
            */

            DateTime last = from;

            int group = 0;

            List<(DateTime from, DateTime to)> downloadIntervals = (await _dBTinkoffContext.DataAboutLoadeds.AsNoTracking()
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

            List<CandlePayload> candleIntervals = downloadIntervals
               .SelectMany(dataTimeInterval => _connection.Context.MarketCandlesAsync(figi, dataTimeInterval.from, dataTimeInterval.to, interval).Result.Candles)
               .ToList();

            await _dBTinkoffContext.Candles.Merge(_mapper.Map<List<EntityCandlePayload>>(candleIntervals), new EntityCandlePayloadEqualityComparer(), new EntityCandlePayloadKeyEqualityComparer());
            await _dBTinkoffContext.SaveChangesAsync();
            //mark DataAboutLoad to load
            return _mapper.Map<List<CandlePayload>>(_dBTinkoffContext.Candles.Where(candle => candle.Time >= from && candle.Time <= to && candle.Figi == figi && candle.Interval == interval).OrderBy(candle => candle.Time));
        }
    }
}
