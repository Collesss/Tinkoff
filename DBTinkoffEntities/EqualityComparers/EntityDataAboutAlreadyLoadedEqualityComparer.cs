using DBTinkoffEntities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBTinkoffEntities.EqualityComparers
{
    public class EntityDataAboutAlreadyLoadedEqualityComparer : IEqualityComparer<EntityDataAboutAlreadyLoaded>
    {
        bool IEqualityComparer<EntityDataAboutAlreadyLoaded>.Equals(EntityDataAboutAlreadyLoaded x, EntityDataAboutAlreadyLoaded y) =>
            x?.Equals(y) ?? false;


        int IEqualityComparer<EntityDataAboutAlreadyLoaded>.GetHashCode(EntityDataAboutAlreadyLoaded obj) =>
            obj?.GetHashCode() ?? 0;
    }
}
