using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DBTinkoff.Repositories.Interfaces
{
    public interface IRepository<T> : IRepositryBase
    {
        Task<IEnumerable<T>> GetAll();
    }
}
