using System.Collections.Generic;
using Tinkoff.Trading.OpenApi.Models;

namespace ConsoleAppTest
{
    public interface ICustomFilter
    {
        public IEnumerable<MarketInstrument> Filtring(IEnumerable<MarketInstrument> entities);
    }
}
