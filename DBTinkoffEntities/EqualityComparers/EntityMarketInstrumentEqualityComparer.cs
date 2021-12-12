using DBTinkoffEntities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBTinkoffEntities.EqualityComparers
{
    class EntityMarketInstrumentEqualityComparer : IEqualityComparer<EntityMarketInstrument>
    {
        bool IEqualityComparer<EntityMarketInstrument>.Equals(EntityMarketInstrument x, EntityMarketInstrument y) =>
            x?.Equals(y) ?? false;


        int IEqualityComparer<EntityMarketInstrument>.GetHashCode(EntityMarketInstrument obj) =>
            obj?.GetHashCode() ?? 0;
    }
}
