using System;
using System.Collections.Generic;
using System.Text;
using Tinkoff.Trading.OpenApi.Models;

namespace DBTinkoffEntities.Entities
{
    public class EntityDataAboutAlreadyLoaded
    {
        public string Figi { get; }
        public DateTime Time { get; }
        public CandleInterval Interval { get; }
        public EntityMarketInstrument Stock { get; set; }

        public EntityDataAboutAlreadyLoaded(string figi, DateTime time, CandleInterval interval)
        {
            Figi = figi;
            Time = time;
            Interval = interval;
        }
    }
}
