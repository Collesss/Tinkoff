using DBTinkoff;
using DBTinkoff.Repositories;
using DBTinkoffEntities.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using Tinkoff.Trading.OpenApi.Models;
using Xunit;

namespace TestProject
{
    public class UnitTestCuttingRequest
    {
        //private readonly Mock<IRepository<EntityCandlePayload>> _mockRepositoryEntityCandlePayload;
        
        private readonly IRepository<EntityCandlePayload> _repositoryCandle;
        private readonly IRepository<EntityMarketInstrument> _repositoryMarket;
        private readonly IRepository<EntityDataAboutAlreadyLoaded> _repositoryDataAboutLoaded;
        private readonly DBTinkoffContext _dBTinkoffContext;
        public UnitTestCuttingRequest()
        {
            _dBTinkoffContext = new DBTinkoffContext(new DbContextOptionsBuilder<DBTinkoffContext>().UseSqlite("Data source=cache.db").Options);

            _repositoryMarket = new Repository<DBTinkoffContext, EntityMarketInstrument>(_dBTinkoffContext);
            _repositoryCandle = new Repository<DBTinkoffContext, EntityCandlePayload>(_dBTinkoffContext);
            _repositoryDataAboutLoaded = new Repository<DBTinkoffContext, EntityDataAboutAlreadyLoaded>(_dBTinkoffContext);

            /*
            _mockRepositoryEntityCandlePayload = new Mock<IRepository<EntityCandlePayload>>();

            _mockRepositoryEntityCandlePayload
                .Setup(obj => obj.GetAll())
                .Returns(new EntityCandlePayload[] { new EntityCandlePayload(0, 0, 0, 0, 0, DateTime.Now, CandleInterval.Hour, "dasd") }.AsQueryable<EntityCandlePayload>());

            */
        }

        [Fact]
        public void Test1()
        {
            DateTime dotStart = DateTime.Now - TimeSpan.FromDays(25);

            DateTime dateTime = DateTime.MinValue;

            TimeSpan check = TimeSpan.FromDays(1);

            Console.WriteLine(dotStart.Date);

            int groupId = 0;

            var values = _repositoryMarket.GetAll()
                .Include(stock => stock.DataAboutLoadeds)
                .Single(stock => stock.Figi == "BBG000HLJ7M4")
                .DataAboutLoadeds
                .Where(dAL => dAL.Time >= dotStart.Date)
                .GroupBy(dAL =>
                {
                    if ((dAL.Time - dateTime.Date) > check)
                        groupId++;
                    
                    dateTime = dAL.Time;

                    return groupId;
                })
                .ToList();
                /*.ToList()
                .ForEach(group =>
                {
                    Console.WriteLine($"group id: {group.Key}; start Time: {group.First().Time}; end Time: {group.Last().Time};");
                });*/

            

            Assert.True(true, "OK");
        }
    }
}
