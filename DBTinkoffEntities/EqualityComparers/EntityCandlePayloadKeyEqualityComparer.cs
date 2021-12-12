using DBTinkoffEntities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBTinkoffEntities.EqualityComparers
{
    public class EntityCandlePayloadKeyEqualityComparer : IEqualityComparer<EntityCandlePayload>
    {
        bool IEqualityComparer<EntityCandlePayload>.Equals(EntityCandlePayload x, EntityCandlePayload y) =>
            x != null &&
            y != null &&
            x.Figi == y.Figi &&
            x.Time == y.Time &&
            x.Interval == y.Interval;

        int IEqualityComparer<EntityCandlePayload>.GetHashCode(EntityCandlePayload obj) =>
            obj.Figi.GetHashCode() ^ (~obj.Interval.GetHashCode() * (int)obj.Time.Ticks);
    }
}
