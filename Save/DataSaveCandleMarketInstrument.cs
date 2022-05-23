using System;
using System.Collections.Generic;
using System.Text;
using Tinkoff.Trading.OpenApi.Models;

namespace Save
{
    public class DataSaveCandleMarketInstrument
    {
        public MarketInstrument MarketInstrument { get; }
        public IEnumerable<CandlePayload> CandlePayload { get; }

        public DataSaveCandleMarketInstrument(MarketInstrument marketInstrument, IEnumerable<CandlePayload> candlePayload)
        {
            MarketInstrument = marketInstrument;
            CandlePayload = candlePayload;
        }
    }
}
