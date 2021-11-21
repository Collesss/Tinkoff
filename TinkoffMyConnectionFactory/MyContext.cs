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

        public MyContext(IConnection<Context> connection) : base(connection) 
        {
            _dataForWait = new Dictionary<string, DataForWait>
                (_settingForRequest.Keys.Select(key => new KeyValuePair<string, DataForWait>
                (key, new DataForWait { DateTimeFirstSendRequest = DateTime.MinValue, RequestCount = 0 })));

            /*
            lock (loker)
            {
                if()
            }
            */
        }


        public new async Task<MarketInstrumentList> MarketStocksAsync()
        {
            lock (loker)
            {
                if (DateTime.Now - _dataForWait["market"].DateTimeFirstSendRequest >= _settingForRequest["market"].MaxRequestTime)
                {
                    _dataForWait["market"].DateTimeFirstSendRequest = DateTime.Now;
                    _dataForWait["market"].RequestCount = 0;
                }

                if (_dataForWait["market"].RequestCount == _settingForRequest["market"].MaxRequestCount)
                {
                    _dataForWait["market"].RequestCount = 0;

                    Task.Delay((_settingForRequest["market"].MaxRequestTime - (DateTime.Now - _dataForWait["market"].DateTimeFirstSendRequest)).Add(TimeSpan.FromSeconds(15))).Wait();
                }

                _dataForWait["market"].RequestCount++;

            }

            return await base.MarketStocksAsync();
        }


        public new async Task<CandleList> MarketCandlesAsync(string figi, DateTime from, DateTime to, CandleInterval interval)
        {
            TinkoffCuterRequest tcr = new TinkoffCuterRequest(interval, from, to);

            List<CandleList> candleLists = new List<CandleList>();

            foreach (var FromToDateTime in tcr)
            {
                lock (loker)
                {
                    if (DateTime.Now - _dataForWait["market"].DateTimeFirstSendRequest >= _settingForRequest["market"].MaxRequestTime)
                    {
                        _dataForWait["market"].DateTimeFirstSendRequest = DateTime.Now;
                        _dataForWait["market"].RequestCount = 0;
                    }

                    if (_dataForWait["market"].RequestCount == _settingForRequest["market"].MaxRequestCount)
                    {
                        _dataForWait["market"].RequestCount = 0;

                        Task.Delay((_settingForRequest["market"].MaxRequestTime - (DateTime.Now - _dataForWait["market"].DateTimeFirstSendRequest)).Add(TimeSpan.FromSeconds(15))).Wait();
                    }

                    _dataForWait["market"].RequestCount++;

                }

                candleLists.Add(await base.MarketCandlesAsync(figi, FromToDateTime.from, FromToDateTime.to, interval));
            }

            return new CandleList(figi, interval, candleLists.SelectMany(el => el.Candles).ToList());
        }
    }
}
