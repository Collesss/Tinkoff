using DBTinkoffEntities.Entities;
using System.Collections.Generic;
using System.Net;

namespace ConsoleAppTest
{
    public class CustomFilter : ICustomFilter
    {
        private readonly string _urlDownLoad;
        public CustomFilter(string urlDownLoad)
        {
            _urlDownLoad = urlDownLoad;
        }

        void ICustomFilter.Filtring(IEnumerable<EntityMarketInstrument> entities)
        {
            new WebClient().DownloadFile(_urlDownLoad, "file.pdf");


        }
    }
}
