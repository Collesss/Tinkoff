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
        private readonly IRepositoryDataAboutAlreadyLoaded _repositoryDataAboutAlreadyLoaded;

        public RepositoryCandlePayload(IConnection<IContext> connection, DBTinkoffContext dBTinkoffContext, 
            IRepositoryDataAboutAlreadyLoaded repositoryDataAboutAlreadyLoaded)
        {
            _connection = connection;
            _dBTinkoffContext = dBTinkoffContext;
            _repositoryDataAboutAlreadyLoaded = repositoryDataAboutAlreadyLoaded;
        }
        async Task<IEnumerable<CandlePayload>> IRepositoryCandlePayload.MarketCandleAsync(string figi, DateTime from, DateTime to, CandleInterval interval)
        {
            IEnumerable<(DateTime from, DateTime to)> downloadIntervals = await _repositoryDataAboutAlreadyLoaded.GetNotLoadIntervals(figi, from, to, interval);
                
            List<CandlePayload> candleIntervals = downloadIntervals
               .SelectMany(dataTimeInterval => _connection.Context.MarketCandlesAsync(figi, dataTimeInterval.from, dataTimeInterval.to, interval).Result.Candles)
               .ToList();

            await _dBTinkoffContext.Candles.Merge(candleIntervals.Select(candle => new EntityCandlePayload(candle)), 
                new EntityCandlePayloadEqualityComparer(), new EntityCandlePayloadKeyEqualityComparer());
            await _dBTinkoffContext.SaveChangesAsync();
            await _repositoryDataAboutAlreadyLoaded.SetLoadInterval(figi, from, to, interval);

            return _dBTinkoffContext.Candles
                .Where(candle => candle.Time >= from && candle.Time <= to && candle.Figi == figi && candle.Interval == interval)
                .OrderBy(candle => candle.Time)
                .Select(candle => 
                    new CandlePayload(candle.Open, candle.Close, candle.High, candle.Low, candle.Volume, candle.Time, candle.Interval, candle.Figi));
        }
    }
}
