using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Save
{
    public interface ISave<T>
    {
        public Task Save(T data);
    }
}
