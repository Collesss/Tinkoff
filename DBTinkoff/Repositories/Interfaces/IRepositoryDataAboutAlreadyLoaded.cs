using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace DBTinkoff.Repositories.Interfaces
{
    public interface IRepositoryDataAboutAlreadyLoaded
    {
        Task<IEnumerable<(DateTime from, DateTime to)>> GetNotLoadIntervals(string figi, DateTime from, DateTime to, CandleInterval interval);

        Task SetLoadInterval(string figi, DateTime from, DateTime to, CandleInterval interval);
    }
}
