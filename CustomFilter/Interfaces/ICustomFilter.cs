using System.Collections.Generic;
using Tinkoff.Trading.OpenApi.Models;

namespace CustomFilter.Interfaces
{
    public interface ICustomFilter
    {
        public IEnumerable<MarketInstrument> Filtring(IEnumerable<MarketInstrument> entities);
    }
}
