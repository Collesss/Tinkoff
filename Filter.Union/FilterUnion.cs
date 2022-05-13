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
        private readonly IUnionNamedDependency<IFilter> _namedDependency;
        private readonly FilterUnionOptions _options;

        public FilterUnion(IUnionNamedDependency<IFilter> namedDependency, IOptions<FilterUnionOptions> options)
        {
            _namedDependency = namedDependency;
            _options = options.Value;
        }

        IEnumerable<MarketInstrument> IFilter.Filtring(IEnumerable<MarketInstrument> entities) =>
            _options.UsingFilter
                .Select(name => _namedDependency.Get(name))
                .Aggregate(entities, (ent, filter) => filter.Filtring(ent));
    }
}
