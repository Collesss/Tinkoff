using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Tinkoff.Trading.OpenApi.Models;

namespace ConsoleAppTinkoffApiTest
{
    public class Data
    {
        public DateTime OpenTime { get; }
        public DateTime CloseTime { get; }
        public decimal Open { get; }
        public decimal Close { get; }
        public decimal High { get; }
        public decimal Low { get; }
        public decimal Volume { get; }

        public Data(DateTime openTime, DateTime closeTime, decimal open, decimal close, decimal low, decimal high, decimal volume)
        {
            OpenTime = openTime;
            CloseTime = closeTime;
            Open = open;
            Close = close;
            Low = low;
            High = high;
            Volume = volume;
        }

        public static Data AgregateCandle(IEnumerable<CandlePayload> candlePayloads)
        {
            candlePayloads = candlePayloads.OrderBy(candle => candle.Time).ToArray();

            return new Data(
                candlePayloads.First().Time, 
                candlePayloads.Last().Time, 
                candlePayloads.First().Open, 
                candlePayloads.Last().Close, 
                candlePayloads.Min(candle => candle.Low), 
                candlePayloads.Max(candle => candle.High), 
                candlePayloads.Average(candle => candle.Volume)
                );
        }
    }
}
