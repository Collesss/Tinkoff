using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace ConsoleAppTinkoffApiTest
{
    public class GetDataContext
    {
        private readonly Context _context;

        private readonly TimeSpan _maxRequestTime;
        private readonly int _maxRequestCount;

        private DateTime _dateTimeFirstSendRequest;
        private int _requestCount;

        private object locker = new object();

        public GetDataContext(Context context, int maxRequestCount, TimeSpan maxRequestTime)
        {
            _context = context;
            _maxRequestTime = maxRequestTime;
            _maxRequestCount = maxRequestCount;
        }

        public IEnumerable<CandlePayload> GetData(string figi, CandleInterval candleInterval, DateTime from, DateTime to)
        {
            TinkoffCuterRequest tcr = new TinkoffCuterRequest(candleInterval, from, to);
            
            return tcr.Select(async dateFromTo =>
            {
                lock (locker)
                {
                    if (_requestCount == _maxRequestCount)
                    {
                        _requestCount = 0;

                        TimeSpan diffWaitTime = DateTime.Now - _dateTimeLastSendRequest;

                        if (_maxRequestTime > diffWaitTime)
                            Task.Delay((_maxRequestTime - diffWaitTime).Add(TimeSpan.FromMilliseconds(50))).Wait();
                    }

                    _requestCount++;
                    _dateTimeLastSendRequest = DateTime.Now;
                }

                return await _context.MarketCandlesAsync(figi, dateFromTo.from, dateFromTo.to, CandleInterval.Hour);
            }).SelectMany(candles => candles.Result.Candles)
            .OrderBy(candle => candle.Time)
            .ToArray();
        }
    }
}
