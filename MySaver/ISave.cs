using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MySaver
{
    public interface ISave<T>
    {
        Task Save(T dataForSave);
    }
}
