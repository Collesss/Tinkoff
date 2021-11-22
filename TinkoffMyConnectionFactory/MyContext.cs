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
        private class DataSettingForRequest
        {
            public TimeSpan MaxRequestTime { get; set; }
            public int MaxRequestCount { get; set; }
        }

        private class DataForWait
        {
            public DateTime DateTimeFirstSendRequest { get; set; }
            public DateTime DateTimeLastSendRequest { get; set; }
            public int RequestCount { get; set; }
        }

        /*
        private static readonly Dictionary<string, (int clientCount, Dictionary<string, (DateTime dateTimeFirstSendRequest, int requestCount)> dataForRestrict)> waitData = 
            new Dictionary<string, (int clientCount, Dictionary<string, (DateTime dateTimeFirstSendRequest, int requestCount)> dataForRestrict)>();
        
        */

        private static readonly Dictionary<string, DataSettingForRequest> _settingForRequest = 
            new Dictionary<string, DataSettingForRequest>()
        {
            ["market"] = new DataSettingForRequest { MaxRequestTime = TimeSpan.FromMinutes(1), MaxRequestCount = 240 }
        };

        private Dictionary<string, DataForWait> _dataForWait;

        private static object loker = new object();

        private ILogger<MyContext> _logger;

        public MyContext(IConnection<Context> connection, ILogger<MyContext> logger) : base(connection) 
        {
            _logger = logger;

            _dataForWait = new Dictionary<string, DataForWait>
                (_settingForRequest.Keys.Select(key => new KeyValuePair<string, DataForWait>
                (key, new DataForWait { DateTimeFirstSendRequest = DateTime.MinValue, DateTimeLastSendRequest = DateTime.MinValue, RequestCount = 0 })));

            /*
            lock (loker)
            {
                if()
            }
            */
        }

        private void UpdateDataWait(string groupRequest)
        {
            lock (loker)
            {
                if (DateTime.Now - _dataForWait[groupRequest].DateTimeFirstSendRequest >= _settingForRequest[groupRequest].MaxRequestTime)
                {
                    _dataForWait[groupRequest].DateTimeFirstSendRequest = DateTime.Now;
                    _dataForWait[groupRequest].RequestCount = 0;
                }

                if (_dataForWait[groupRequest].RequestCount == _settingForRequest[groupRequest].MaxRequestCount)
                {
                    _dataForWait[groupRequest].RequestCount = 0;

                    TimeSpan waitTime = (_settingForRequest[groupRequest].MaxRequestTime - (DateTime.Now - _dataForWait[groupRequest].DateTimeLastSendRequest)).Add(TimeSpan.FromSeconds(15));

                    _logger.LogInformation($"start wait: {waitTime}");
                    
                    Task.Delay(waitTime).Wait();
                }

                _dataForWait[groupRequest].RequestCount++;

                _dataForWait[groupRequest].DateTimeLastSendRequest = DateTime.Now;

            }
        }

        public new async Task<MarketInstrumentList> MarketStocksAsync()
        {
            UpdateDataWait("market");

            MarketInstrumentList result;

            try
            {
                _logger.LogInformation($"{_dataForWait["market"].RequestCount}: send request Market Stoks: {DateTime.Now}");
                result = await base.MarketStocksAsync();
                _logger.LogInformation($"{_dataForWait["market"].RequestCount}: get data request Market Stoks: {DateTime.Now}");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"fock: {e.Message}");

                throw;
            }

            return result;
        }


        public new async Task<CandleList> MarketCandlesAsync(string figi, DateTime from, DateTime to, CandleInterval interval)
        {
            TinkoffCuterRequest tcr = new TinkoffCuterRequest(interval, from, to);

            List<CandleList> candleLists = new List<CandleList>();

            foreach (var FromToDateTime in tcr)
            {
                UpdateDataWait("market");

                CandleList result;

                try
                {
                    _logger.LogInformation($"{_dataForWait["market"].RequestCount}: send request Market Candles {figi}: {DateTime.Now}");
                    result = await base.MarketCandlesAsync(figi, FromToDateTime.from, FromToDateTime.to, interval);
                    _logger.LogInformation($"{_dataForWait["market"].RequestCount}: get data request Market Candles {figi}: {DateTime.Now}");
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, $"{_dataForWait["market"].RequestCount}: very fock: {e.Message}");

                    throw;
                }

                candleLists.Add(result);
            }

            return new CandleList(figi, interval, candleLists.SelectMany(el => el.Candles).ToList());
        }
    }
}
