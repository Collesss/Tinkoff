using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Tinkoff.Trading.OpenApi.Models;

namespace ConsoleAppTest.Transform
{
    public class Transform : ITransform<IEnumerable<CandlePayload>, IEnumerable<Data>>
    {
        IEnumerable<Data> ITransform<IEnumerable<CandlePayload>, IEnumerable<Data>>.Transform(IEnumerable<CandlePayload> from) =>
            from
            .GroupBy(el => $"{el.Time.Year}|{el.Time.Month}|{el.Time.Day}|{(el.Time.Hour + 1) / 4}")
            .Select(group => Data.AgregateCandle(group))
            .OrderBy(aggCandle => aggCandle.OpenTime);
    }
}
