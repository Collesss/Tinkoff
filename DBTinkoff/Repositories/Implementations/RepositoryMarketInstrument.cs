using AutoMapper;
using DBTinkoff.Repositories.Interfaces;
using DBTinkoffEntities.Entities;
using DBTinkoffEntities.EqualityComparers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace DBTinkoff.Repositories.Implementations
{
    public class RepositoryMarketInstrument : IRepositoryMarketInstrument
    {
        private readonly IConnection<IContext> _connection;
        private readonly DBTinkoffContext _dBTinkoffContext;

        public RepositoryMarketInstrument(IConnection<IContext> connection, DBTinkoffContext dBTinkoffContext)
        {
            _connection = connection;
            _dBTinkoffContext = dBTinkoffContext;
        }

        async Task<IEnumerable<MarketInstrument>> IRepositoryMarketInstrument.GetAllAsync()
        {
            MarketInstrumentList marketInstrumentList = await _connection.Context.MarketBondsAsync();

            await _dBTinkoffContext.Stoks.Merge(marketInstrumentList.Instruments.Select(stock => new EntityMarketInstrument(stock)), 
                new EntityMarketInstrumentKeyEqualityComparer(), 
                new EntityMarketInstrumentKeyEqualityComparer());

            await _dBTinkoffContext.SaveChangesAsync();

            return marketInstrumentList.Instruments;
        }
    }
}
