﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tinkoff.Trading.OpenApi.Models;

namespace Filter.Union
{
    public class FilterUnion : IFilterUnion
    {
        private readonly IEnumerable<IFilter> _filters;

        public FilterUnion(IEnumerable<IFilter> filters)
        {
            _filters = filters;
        }

        IEnumerable<MarketInstrument> IFilter.Filtring(IEnumerable<MarketInstrument> entities) =>
            _filters.Aggregate(entities, (ent, filter) => filter.Filtring(ent));
    }
}