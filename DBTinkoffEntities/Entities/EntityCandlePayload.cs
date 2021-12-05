using System;
using Tinkoff.Trading.OpenApi.Models;

namespace DBTinkoffEntities.Entities
{
    public class EntityCandlePayload : CandlePayload
    {
        public EntityMarketInstrument Stock { get; set; }

        public EntityCandlePayload(decimal open, decimal close, decimal high, decimal low, decimal volume, DateTime time, CandleInterval interval, string figi)
            : base(open, close, high, low, volume, time, interval, figi) { }
    }
}
