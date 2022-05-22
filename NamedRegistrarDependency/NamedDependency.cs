using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace NamedRegistrarDependency
{
    public class NamedDependency<T, V> : INamedDependency<T> where V: T
    {
        string INamedDependency<T>.Name => _name;

        T INamedDependency<T>.Dependency => _dependency;

        private readonly string _name;
        private readonly T _dependency;

        public NamedDependency(IOptions<OptionNamedType<V>> optionNamedType, V dependency)
        {
            _name = optionNamedType.Value.name;
            _dependency = dependency;
        }
    }
}
