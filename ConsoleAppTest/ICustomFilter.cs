using DBTinkoffEntities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppTest
{
    public interface ICustomFilter
    {
        public void Filtring(IEnumerable<EntityMarketInstrument> entities);
    }
}
