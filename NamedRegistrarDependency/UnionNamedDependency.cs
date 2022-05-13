using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NamedRegistrarDependency
{
    public class UnionNamedDependency<T> : IUnionNamedDependency<T>
    {
        private readonly IEnumerable<INamedDependency<T>> _namedDependencies;

        public UnionNamedDependency(IEnumerable<INamedDependency<T>> namedDependencies)
        {
            _namedDependencies = namedDependencies;
        }

        T IUnionNamedDependency<T>.Get(string name) =>
            _namedDependencies.Single(depend => depend.Name == name).Dependency;
    }
}
