using DBTinkoffEntities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBTinkoffEntities.EqualityComparers
{
    public class EntityCandlePayloadEqualityComparer : IEqualityComparer<EntityCandlePayload>
    {
        bool IEqualityComparer<EntityCandlePayload>.Equals(EntityCandlePayload x, EntityCandlePayload y) =>
            x?.Equals(y) ?? false;
        

        int IEqualityComparer<EntityCandlePayload>.GetHashCode(EntityCandlePayload obj) =>
            obj?.GetHashCode() ?? 0;
    }
}
