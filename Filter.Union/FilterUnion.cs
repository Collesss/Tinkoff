using Microsoft.Extensions.Options;
using NamedRegistrarDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tinkoff.Trading.OpenApi.Models;

namespace Filter.Union
{
    public class FilterUnion : IFilter
    {
        private readonly IEnumerable<IFilter> _filters;

        public FilterUnion(IEnumerable<INamedDependency<IFilter>> filters)
        {
            _filters = filters.Select(namedDep => namedDep.Dependency);
        }

        IEnumerable<MarketInstrument> IFilter.Filtring(IEnumerable<MarketInstrument> entities) =>
            _filters.Aggregate(entities, (ent, filter) => filter.Filtring(ent));
    }
}
