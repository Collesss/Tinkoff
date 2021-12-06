using DBTinkoff.Repositories;
using DBTinkoffEntities.Entities;
using Moq;
using System;
using System.Linq;
using Tinkoff.Trading.OpenApi.Models;
using Xunit;

namespace TestProject
{
    public class UnitTestCuttingRequest
    {
        private readonly Mock<IRepository<EntityCandlePayload>> _mockRepositoryEntityCandlePayload;

        public UnitTestCuttingRequest()
        {
            _mockRepositoryEntityCandlePayload = new Mock<IRepository<EntityCandlePayload>>();

            _mockRepositoryEntityCandlePayload
                .Setup(obj => obj.GetAll())
                .Returns(new EntityCandlePayload[] { new EntityCandlePayload(0, 0, 0, 0, 0, DateTime.Now, CandleInterval.Hour, "dasd") }.AsQueryable<EntityCandlePayload>());
        }

        [Fact]
        public void Test1()
        {


        }
    }
}
