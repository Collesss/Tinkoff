using AutoMapper;
using DBTinkoffEntities.Entities;
using DBTinkoffEntities.EqualityComparers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace DBTinkoff.Repositories.Interfaces
{
    public class RepositoryMarketInstrument : IRepositoryMarketInstrument
    {
        private readonly IConnection<IContext> _connection;
        private readonly DBTinkoffContext _dBTinkoffContext;
        private readonly IMapper _mapper;

        public RepositoryMarketInstrument(IConnection<IContext> connection, DBTinkoffContext dBTinkoffContext, IMapper mapper)
        {
            _connection = connection;
            _dBTinkoffContext = dBTinkoffContext;
            _mapper = mapper;
        }

        async Task<IEnumerable<MarketInstrument>> IRepositoryMarketInstrument.GetAllAsync()
        {
            MarketInstrumentList marketInstrumentList = await _connection.Context.MarketBondsAsync();

            await _dBTinkoffContext.Stoks.Merge(_mapper.Map<List<EntityMarketInstrument>>(marketInstrumentList.Instruments), 
                new EntityMarketInstrumentKeyEqualityComparer(), 
                new EntityMarketInstrumentKeyEqualityComparer());

            await _dBTinkoffContext.SaveChangesAsync();

            return marketInstrumentList.Instruments;
        }
    }
}
