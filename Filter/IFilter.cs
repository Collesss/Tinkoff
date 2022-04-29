using System.Collections.Generic;
using Tinkoff.Trading.OpenApi.Models;

namespace Filter
{
    public interface IFilter
    {
        public IEnumerable<MarketInstrument> Filtring(IEnumerable<MarketInstrument> entities);
    }
}
