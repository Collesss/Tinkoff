using System;
using System.Collections.Generic;
using System.Text;

namespace NamedRegistrarDependency
{
    public class NamedDependency<T> : INamedDependency<T>
    {
        string INamedDependency<T>.Name => _name;

        T INamedDependency<T>.Dependency => _dependency;

        private readonly string _name;
        private readonly T _dependency;

        public NamedDependency(string name, T dependency)
        {
            _name = name;
            _dependency = dependency;
        }
    }
}
