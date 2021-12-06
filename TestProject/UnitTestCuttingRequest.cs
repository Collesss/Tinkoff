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
        private readonly DBTinkoffContext _dBTinkoffContext;
        public UnitTestCuttingRequest()
        {
            _dBTinkoffContext = new DBTinkoffContext(new DbContextOptionsBuilder<DBTinkoffContext>().UseSqlite("Data source=test.db").Options);

            _repositoryMarket = new Repository<DBTinkoffContext, EntityMarketInstrument>(_dBTinkoffContext);
            _repositoryCandle = new Repository<DBTinkoffContext, EntityCandlePayload>(_dBTinkoffContext);

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
            DateTime dateTime = DateTime.MinValue;

            TimeSpan check = TimeSpan.FromDays(1);

            int group = 1;

            _repositoryMarket.GetAll().FirstOrDefault().Candles
                .OrderBy(candle => candle.Time)
                .GroupBy(candle => 
                {
                    if ((dateTime.Date - candle.Time.Date) > check)
                    {
                        dateTime = candle.Time;
                        group++;
                    }

                    return group;
                });

            Assert.True(true, "OK");
        }
    }
}
