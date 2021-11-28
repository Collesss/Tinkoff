using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MySaver
{
    public interface ISave<T>
    {
        Task Save<V>(T metaDataForSave, IEnumerable<V> elements, IEnumerable<(Func<V, object> element, string header, string format)> columns);
    }
}
