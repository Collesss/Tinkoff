using DBTinkoff;
using DBTinkoff.Repositories;
using DBTinkoffEntities.Entities;
using DBTinkoffEntities.EqualityComparers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySaver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using TinkoffMyConnectionFactory;

namespace ConsoleAppTest
{
    public class MyMain
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection<IContext> _connection;
        //private readonly IRepository<EntityCandlePayload> _repositoryCandle;
        //private readonly IRepository<EntityMarketInstrument> _repositoryMarket;
        private readonly ISave<(string fileName, string sheetName)> _save;
        private readonly ILogger<MyMain> _logger;
        private readonly int _days;

        public MyMain(IServiceProvider serviceProvider, IConnection<IContext> connection, ISave<(string fileName, string sheetName)> save, ILogger<MyMain> logger, int days)
        {
            _serviceProvider = serviceProvider;
            _connection = connection;
            //_repositoryCandle = repositoryCandle;
            //_repositoryMarket = repositoryMarket;
            _save = save;
            _logger = logger;
            _days = days;
        }

        public async Task Main(CancellationToken cancellationToken)
        {
            //IServiceScope scope = _serviceProvider.CreateScope();

            IContext context = _connection.Context;

            //var stocks = await context.MarketStocksAsync();

            List<EntityMarketInstrument> stocks;
            
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IRepository<EntityMarketInstrument> repositoryStock = scope.ServiceProvider.GetRequiredService<IRepository<EntityMarketInstrument>>();

                var noTrakStocks = await repositoryStock.GetAll().AsNoTracking().ToListAsync();

                var CreateAndUpdate = (await context.MarketStocksAsync())
                    .Instruments
                    .Select(stock => new EntityMarketInstrument(stock))
                    .Except(noTrakStocks, new CommonEqualityComparer<EntityMarketInstrument>())
                    .ToList();

                var Create = CreateAndUpdate.Except(noTrakStocks, new EntityMarketInstrumentKeyEqualityComparer()).ToList();
                var Update = CreateAndUpdate.Except(Create, new EntityMarketInstrumentKeyEqualityComparer()).ToList();

                await repositoryStock.CreateAsync(Create);
                await repositoryStock.UpdateAsync(Update);

                stocks = await repositoryStock.GetAll()
                    .ToListAsync();
            }

            //DateTime dateTimeStart = (DateTime.Now - TimeSpan.FromDays(_days)).Date;
            //DateTime dateTimeEnd = (DateTime.Now + TimeSpan.FromDays(1)).Date;
            
            foreach (var stock in stocks)
                _logger.LogInformation($"{stock.Name}: {stock.Figi}: {stock.Ticker};");

            _logger.LogInformation(stocks.Count.ToString());

            Regex regex = new Regex(@"[\\\/:*?""<>|]");

            int i = 1;

            DateTime start = DateTime.UtcNow.Date.AddDays(-_days).Date;
            DateTime end = DateTime.UtcNow.Date.AddDays(1);
            
            foreach (var stock in stocks)
            {
                List<EntityDataAboutAlreadyLoaded> dAL = new List<EntityDataAboutAlreadyLoaded>();

                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    var contextTinkoff = scope.ServiceProvider.GetRequiredService<DBTinkoffContext>();

                    //contextTinkoff.Candles.AddOrUpdate();
                    
                    contextTinkoff.Attach(stock);

                    /*
                    contextTinkoff.Entry(stock)
                        .Collection(s => s.Candles)
                        .Query()
                        .Where(c => c.Time > start)
                        .Load();
                    */

                    contextTinkoff.Entry(stock)
                        .Collection(s => s.DataAboutLoadeds)
                        .Query()
                        .Where(d => d.Time >= start)
                        .Load();
                }

                if (cancellationToken.IsCancellationRequested)
                    break;
                /*
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                    if (consoleKeyInfo.Key == ConsoleKey.C && (consoleKeyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                        break;
                }
                */

                DateTime dateTimeLast = start.AddDays(-1);

                var dataAboutLoaded = stock
                    .DataAboutLoadeds
                    .Where(dAL => dAL.Time > start)
                    .Select(dAL => dAL.Time)
                    .OrderBy(time => time)
                    .Append(end);

                List<(DateTime start, DateTime end)> rangesQueries = new List<(DateTime start, DateTime end)>();

                TimeSpan check = TimeSpan.FromDays(1);

                foreach (DateTime time in dataAboutLoaded)
                {
                    DateTime utcTime = new DateTime(time.Ticks, DateTimeKind.Utc);

                    if ((utcTime - dateTimeLast) > check)
                        rangesQueries.Add((start: dateTimeLast.AddDays(1), end: utcTime));
                    dateTimeLast = utcTime;
                }

                
                IEnumerable<EntityCandlePayload> entityCandlePayloads = rangesQueries
                    .SelectMany(range => context.MarketCandlesAsync(stock.Figi, range.start, range.end, CandleInterval.Hour).Result.Candles)
                    .Select(candle => new EntityCandlePayload(candle))
                    .ToList();

                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    var stockCandles = await scope.ServiceProvider.GetRequiredService<DBTinkoffContext>()
                        .Attach(stock)
                        .Collection(stock => stock.Candles)
                        .Query()
                        .AsNoTracking()
                        .Where(c => c.Time > start)
                        .ToListAsync();

                    var CreateAndUpdateCandle = entityCandlePayloads.Except(stockCandles, new CommonEqualityComparer<EntityCandlePayload>()).ToList();

                    var CreateCandle = CreateAndUpdateCandle.Except(stockCandles, new EntityCandlePayloadKeyEqualityComparer()).ToList();
                    var UpdateCandle = CreateAndUpdateCandle.Except(CreateCandle, new EntityCandlePayloadKeyEqualityComparer()).ToList();

                    var repositoryCandle = scope.ServiceProvider.GetRequiredService<IRepository<EntityCandlePayload>>();

                    await repositoryCandle.CreateAsync(CreateCandle);
                    await repositoryCandle.UpdateAsync(UpdateCandle);


                    /*
                    var t = Enumerable.Range(0, _days)
                        .Select(i => new EntityDataAboutAlreadyLoaded(stock.Figi, start.AddDays(i).Date, CandleInterval.Hour))
                        .Except(stock.DataAboutLoadeds, new CommonEqualityComparer<EntityDataAboutAlreadyLoaded>());
                    */

                    await scope.ServiceProvider.GetRequiredService<IRepository<EntityDataAboutAlreadyLoaded>>()
                        .CreateAsync(Enumerable.Range(0, _days)
                        .Select(i => new EntityDataAboutAlreadyLoaded(stock.Figi, start.AddDays(i).Date, CandleInterval.Hour))
                        .Except(stock.DataAboutLoadeds, new CommonEqualityComparer<EntityDataAboutAlreadyLoaded>()));

                    scope.ServiceProvider.GetRequiredService<DBTinkoffContext>().Attach(stock);
                }
                
                
                var candles = stock.Candles
                    .Where(candle => candle.Time > start)
                    .GroupBy(el => $"{el.Time.Year}|{el.Time.Month}|{el.Time.Day}|{(el.Time.Hour + 1) / 4}")
                    .Select(group => Data.AgregateCandle(group))
                    .OrderBy(aggCandle => aggCandle.OpenTime);

                await _save.Save(($"{regex.Replace(stock.Name, match => "_")}.xlsx", "Data"), candles, new (Func<Data, object> element, string header, string format)[]
                {
                    (d => d.CloseTime, "CloseTime", "dd.MM.yyyy HH:mm"),
                    (d => d.Open, "Open", null),
                    (d => d.Close, "Close", null),
                    (d => d.Low, "Low", null),
                    (d => d.High, "High", null)
                });

                _logger.LogInformation($"Figi:{stock.Figi} Name:{stock.Name}; {i}/{stocks.Count()}");

                i++;
            }
        }
    }
}
