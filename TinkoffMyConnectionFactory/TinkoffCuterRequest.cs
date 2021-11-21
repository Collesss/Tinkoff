using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tinkoff.Trading.OpenApi.Models;

namespace TinkoffMyConnectionFactory
{
    public class TinkoffCuterRequest : IEnumerable<(DateTime from, DateTime to)>
    {
        public CandleInterval CandleInterval { get; }
        public int CountRequest { get; }
        private readonly DateTime _from;
        private readonly DateTime _to;

        //CandleInterval - interval, int - days for interval
        private static readonly Dictionary<CandleInterval, int> _daysForIntervals = new Dictionary<CandleInterval, int>
        {
            [CandleInterval.Minute] = 1,
            [CandleInterval.TwoMinutes] = 1,
            [CandleInterval.ThreeMinutes] = 1,
            [CandleInterval.FiveMinutes] = 1,
            [CandleInterval.TenMinutes] = 1,
            [CandleInterval.QuarterHour] = 1,
            [CandleInterval.HalfHour] = 1,
            [CandleInterval.Hour] = 7,
            [CandleInterval.Day] = 365,
            [CandleInterval.Week] = 730,
            [CandleInterval.Month] = 36500
        };

        /*
        Интервал свечи и допустимый промежуток запроса:
        - 1min [1 minute, 1 day]
        - 2min [2 minutes, 1 day]
        - 3min [3 minutes, 1 day]
        - 5min [5 minutes, 1 day]
        - 10min [10 minutes, 1 day]
        - 15min [15 minutes, 1 day]
        - 30min [30 minutes, 1 day]
        - hour [1 hour, 7 days]
        - day [1 day, 1 year]
        - week [7 days, 2 years]
        - month [1 month, 10 years]
        */

        public TinkoffCuterRequest(CandleInterval candleInterval, DateTime from, DateTime to)
        {
            if (from > to)
                throw new ArgumentException("to dont less from", "from");

            CountRequest = (int) Math.Ceiling((to - from).TotalDays / _daysForIntervals[candleInterval]);
            CandleInterval = candleInterval;
            _from = from;
            _to = to;
        }

        public IEnumerator<(DateTime from, DateTime to)> GetEnumerator()
        {
            DateTime from = _from;

            while (from < _to)
                yield return (from: from, to: (from += TimeSpan.FromDays(_daysForIntervals[CandleInterval])) < _to ? from : _to);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            DateTime from = _from;

            while (from < _to)
                yield return (from: from, to: (from += TimeSpan.FromDays(_daysForIntervals[CandleInterval])) < _to ? from : _to);
        }
    }
}
