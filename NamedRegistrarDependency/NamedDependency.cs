using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace NamedRegistrarDependency
{
    public class NamedDependency<T> : INamedDependency<T>
    {
        string INamedDependency<T>.Name => _name;

        T INamedDependency<T>.Dependency => _dependency;

        private readonly string _name;
        private readonly T _dependency;

        public NamedDependency(IOptions<OptionNamedType<T>> optionNamedType, T dependency)
        {
            _name = optionNamedType.Value.name;
            _dependency = dependency;
        }
    }
}
