using System;
using System.Collections.Generic;
using System.Text;
using Tinkoff.Trading.OpenApi.Models;

namespace DBTinkoffEntities.Entities
{
    public class EntityDataAboutAlreadyLoaded : DataAboutAlreadyLoaded, IEquatable<EntityDataAboutAlreadyLoaded>
    {
        public EntityMarketInstrument Stock { get; set; }

        public EntityDataAboutAlreadyLoaded(string figi, DateTime time, CandleInterval interval) : base(figi, time, interval) { }

        bool IEquatable<EntityDataAboutAlreadyLoaded>.Equals(EntityDataAboutAlreadyLoaded other) =>
            other != null &&
            this.Figi == other.Figi &&
            this.Time == other.Time &&
            this.Interval == other.Interval;

        public override bool Equals(object obj) =>
            obj is EntityDataAboutAlreadyLoaded dataLoaded &&
            ((IEquatable<EntityDataAboutAlreadyLoaded>)this).Equals(dataLoaded);

        public override int GetHashCode() =>
            ((int)(Time.Ticks * 2) + (~(int)Interval)) * Figi.GetHashCode();
    }
}
