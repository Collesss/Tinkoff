using System;
using System.Collections.Generic;
using System.Text;

namespace NamedRegistrarDependency
{
    public interface INamedDependency<T>
    {
        string Name { get; }
        T Dependency { get; }
    }
}
