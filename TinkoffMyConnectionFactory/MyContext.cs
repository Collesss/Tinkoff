using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace TinkoffMyConnectionFactory
{
    public class MyContext : Context, IContext
    {
        private ILogger<MyContext> _logger;

        public MyContext(IConnection<Context> connection, ILogger<MyContext> logger) : base(connection) 
        {
            _logger = logger;
        }

        public new async Task<MarketInstrumentList> MarketStocksAsync()
        {
            MarketInstrumentList result = null;

            do
            {
                try
                {
                    _logger.LogInformation($"send request Market Stoks: {DateTime.Now}");
                    result = await base.MarketStocksAsync();
                    _logger.LogInformation($"get data request Market Stoks: {DateTime.Now}");
                }
                catch (OpenApiException e)
                {
                    _logger.LogError(e, $"not very fock: {e.Message}");
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, $"fock: {e.Message}");

                    throw;
                }
            }
            while (result == null);

            return result;
        }


        public new async Task<CandleList> MarketCandlesAsync(string figi, DateTime from, DateTime to, CandleInterval interval)
        {
            TinkoffCuterRequest tcr = new TinkoffCuterRequest(interval, from, to);

            List<CandleList> candleLists = new List<CandleList>();

            foreach (var FromToDateTime in tcr)
            {
                CandleList result = null;

                do
                {
                    try
                    {
                        _logger.LogInformation($"send request Market Candles {figi}: {DateTime.Now}");
                        result = await base.MarketCandlesAsync(figi, FromToDateTime.from, FromToDateTime.to, interval);
                        _logger.LogInformation($"get data request Market Candles {figi}: {DateTime.Now}");
                    }
                    catch (OpenApiException e)
                    {
                        _logger.LogError(e, $"not very fock: {e.Message}");
                        await Task.Delay(TimeSpan.FromMinutes(1));
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e, $"very fock: {e.Message}");

                        throw;
                    }
                }
                while (result == null);

                candleLists.Add(result);
            }

            return new CandleList(figi, interval, candleLists.SelectMany(el => el.Candles).ToList());
        }
    }
}
