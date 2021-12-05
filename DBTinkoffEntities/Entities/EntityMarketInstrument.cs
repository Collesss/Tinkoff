using System.Collections.Generic;
using Tinkoff.Trading.OpenApi.Models;

namespace DBTinkoffEntities.Entities
{
    public class EntityMarketInstrument : MarketInstrument
    {
        public List<EntityCandlePayload> Candles { get; set; } = new List<EntityCandlePayload>();

        public EntityMarketInstrument(string figi, string ticker, string isin, decimal minPriceIncrement, int lot, Currency currency, string name, InstrumentType type)
            : base(figi, ticker, isin, minPriceIncrement, lot, currency, name, type) { }
    }
}
