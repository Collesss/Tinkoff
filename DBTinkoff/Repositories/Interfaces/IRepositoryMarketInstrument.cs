using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace DBTinkoff.Repositories.Interfaces
{
    public interface IRepositoryMarketInstrument
    {
        Task<IEnumerable<MarketInstrument>> GetAllAsync();
    }
}
