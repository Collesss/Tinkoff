using DBTinkoffEntities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBTinkoffEntities.EqualityComparers
{
    public class EntityMarketInstrumentKeyEqualityComparer : IEqualityComparer<EntityMarketInstrument>
    {
        bool IEqualityComparer<EntityMarketInstrument>.Equals(EntityMarketInstrument x, EntityMarketInstrument y) =>
            x != null &&
            y != null &&
            x.Figi == y.Figi &&
            x.Ticker == y.Ticker &&
            x.Isin == y.Isin;

        int IEqualityComparer<EntityMarketInstrument>.GetHashCode(EntityMarketInstrument obj) =>
            obj.Figi.GetHashCode()^(~obj.Ticker.GetHashCode()*obj.Isin.GetHashCode());
    }
}
