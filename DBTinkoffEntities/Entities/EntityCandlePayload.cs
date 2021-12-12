using System;
using Tinkoff.Trading.OpenApi.Models;

namespace DBTinkoffEntities.Entities
{
    public class EntityCandlePayload : CandlePayload, IEquatable<EntityCandlePayload>
    {
        public EntityMarketInstrument Stock { get; set; }

        public EntityCandlePayload(decimal open, decimal close, decimal high, decimal low, decimal volume, DateTime time, CandleInterval interval, string figi)
            : base(open, close, high, low, volume, time, interval, figi) { }
    
        public EntityCandlePayload(CandlePayload candle) 
            : base(candle.Open, candle.Close, candle.High, candle.Low, candle.Volume, candle.Time, candle.Interval, candle.Figi) { }

        bool IEquatable<EntityCandlePayload>.Equals(EntityCandlePayload other) =>
            other           != null && 
            this.Close      == other.Close &&
            this.Open       == other.Open &&
            this.Low        == other.Low &&
            this.High       == other.High &&
            this.Volume     == other.Volume &&
            this.Time       == other.Time &&
            this.Figi       == other.Figi &&
            this.Interval   == other.Interval;

        public override bool Equals(object obj) =>
            obj is EntityCandlePayload candle && 
            ((IEquatable<EntityCandlePayload>)this).Equals(candle);

        public override int GetHashCode() =>
            ((int)(Close * 2 + Open * 3 + Low * 4 + High * 5 + Volume * 6) + (int)(Time.Ticks * 7) + (~(int)Interval)) * Figi.GetHashCode();
    }
}
