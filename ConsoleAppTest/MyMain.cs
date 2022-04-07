using ConsoleAppTest.Transform;
using DBTinkoff;
using DBTinkoff.Repositories;
using DBTinkoff.Repositories.Interfaces;
using DBTinkoffEntities.Entities;
using DBTinkoffEntities.EqualityComparers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySaver;
using MySaver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace ConsoleAppTest
{
    public class MyMain
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISave<SaveExcelData<Data>> _save;
        private readonly ILogger<MyMain> _logger;
        private readonly ICustomFilter _customFilter;
        private readonly IOptions<Options> _options;
        private readonly IRepositoryMarketInstrument _repositoryMarketInstrument;
        private readonly IRepositoryCandlePayload _repositoryCandlePayload;
        private readonly ITransform<IEnumerable<CandlePayload>, IEnumerable<Data>> _transform;

        public MyMain(IServiceProvider serviceProvider,
            ISave<SaveExcelData<Data>> save, 
            ILogger<MyMain> logger, 
            ICustomFilter customFilter,
            IRepositoryMarketInstrument repositoryMarketInstrument,
            IRepositoryCandlePayload repositoryCandlePayload,
            IOptions<Options> options)
        {
            _serviceProvider = serviceProvider;
            _save = save;
            _logger = logger;
            _customFilter = customFilter;
            _repositoryMarketInstrument = repositoryMarketInstrument;
            _repositoryCandlePayload = repositoryCandlePayload;
            _options = options;
        }

        public async Task Main(CancellationToken cancellationToken)
        {
            IEnumerable<MarketInstrument> stocks = await _repositoryMarketInstrument.GetAllAsync();

            foreach (var stock in stocks)
                _logger.LogInformation($"{stock.Name}: {stock.Figi}: {stock.Ticker};");

            _logger.LogInformation(stocks.Count().ToString());

            Regex regex = new Regex(@"[\\\/:*?""<>|]");

            int i = 1;

            DateTime start = DateTime.UtcNow.Date.AddDays(-_options.Value.Days).Date;
            DateTime end = DateTime.UtcNow.Date.AddDays(1);

            if (_options.Value.CustomFilter)
                stocks = _customFilter.Filtring(stocks).ToList();

            stocks = stocks.ToList();

            Console.WriteLine(stocks.Count());

            foreach (var stock in stocks)
            {
                //List<EntityDataAboutAlreadyLoaded> dAL = new List<EntityDataAboutAlreadyLoaded>();
                
                if (cancellationToken.IsCancellationRequested)
                    break;
                /*
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
                */

                var candles = _transform.Transform(await _repositoryCandlePayload.MarketCandleAsync("figi", start, end, CandleInterval.Hour));

                await _save.Save(new SaveExcelData<Data>($"{regex.Replace(stock.Name, match => "_")}.xlsx", "Data", candles));

                _logger.LogInformation($"Figi:{stock.Figi} Name:{stock.Name}; {i}/{stocks.Count()}");

                i++;
            }
        }
    }
}
