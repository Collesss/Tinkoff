using DBTinkoff.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace ConsoleAppTest
{
    public class MyMain
    {
        private readonly ISaveCandleMarketInstrument _save;
        private readonly ILogger<MyMain> _logger;
        private readonly IOptions<Options> _options;
        private readonly IRepositoryMarketInstrument _repositoryMarketInstrument;
        private readonly IRepositoryCandlePayload _repositoryCandlePayload;

        public MyMain(
            ISaveCandleMarketInstrument save, 
            ILogger<MyMain> logger,
            IRepositoryMarketInstrument repositoryMarketInstrument,
            IRepositoryCandlePayload repositoryCandlePayload,
            IOptions<Options> options)
        {
            _save = save;
            _logger = logger;
            _repositoryMarketInstrument = repositoryMarketInstrument;
            _repositoryCandlePayload = repositoryCandlePayload;
            _options = options;
        }

        public async Task Main(CancellationToken cancellationToken)
        {
            IEnumerable<MarketInstrument> stocks = await _repositoryMarketInstrument.GetAllAsync();

            foreach (var stock in stocks)
                _logger.LogInformation($"{stock.Name}: {stock.Figi}: {stock.Ticker};");

            _logger.LogInformation($"total stokcs: {stocks.Count()}");

            int i = 1;

            DateTime start = DateTime.UtcNow.Date.AddDays(-_options.Value.Days).Date;
            DateTime end = DateTime.UtcNow.Date.AddDays(1);

            _logger.LogInformation(stocks.Count().ToString());

            foreach (var stock in stocks)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var candels = await _repositoryCandlePayload.MarketCandleAsync(stock.Figi, start, end, CandleInterval.Hour);

                await _save.Save(new DataSaveCandleMarketInstrument(stock, candels));

                _logger.LogInformation($"Figi:{stock.Figi} Name:{stock.Name}; {i}/{stocks.Count()}");

                i++;
            }
        }
    }
}
