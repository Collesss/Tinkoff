using System;
using System.Collections.Generic;
using Tinkoff.Trading.OpenApi.Models;

namespace DBTinkoffEntities.Entities
{
    public class EntityMarketInstrument : MarketInstrument, IEquatable<EntityMarketInstrument>
    {
        public List<EntityCandlePayload> Candles { get; set; } = new List<EntityCandlePayload>();
        public List<EntityDataAboutAlreadyLoaded> DataAboutLoadeds { get; set; } = new List<EntityDataAboutAlreadyLoaded>();

        public EntityMarketInstrument(string figi, string ticker, string isin, decimal minPriceIncrement, int lot, Currency currency, string name, InstrumentType type)
            : base(figi, ticker, isin, minPriceIncrement, lot, currency, name, type) { }

        public EntityMarketInstrument(MarketInstrument stock)
            : base(stock.Figi, stock.Ticker, stock.Isin, stock.MinPriceIncrement, stock.Lot, stock.Currency, stock.Name, stock.Type) { }

        bool IEquatable<EntityMarketInstrument>.Equals(EntityMarketInstrument other) =>
            other != null &&
            this.Figi == other.Figi &&
            this.Ticker == other.Ticker &&
            this.Isin == other.Isin &&
            this.MinPriceIncrement == other.MinPriceIncrement &&
            this.Lot == other.Lot &&
            this.Currency == other.Currency &&
            this.Name == other.Name &&
            this.Type == other.Type;

        public override bool Equals(object obj) =>
            obj is EntityMarketInstrument stock &&
            ((IEquatable<EntityMarketInstrument>)this).Equals(stock);

        public override int GetHashCode() =>
            (int)(MinPriceIncrement * 2) + Lot * 3 + (~(int)Currency * ~(int)Type) * Name.GetHashCode() * Figi.GetHashCode() * Ticker.GetHashCode() * Isin.GetHashCode();
    }
}
