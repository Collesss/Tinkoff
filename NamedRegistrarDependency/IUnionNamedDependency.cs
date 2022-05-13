using System;
using System.Collections.Generic;
using System.Text;

namespace NamedRegistrarDependency
{
    public interface IUnionNamedDependency<T>
    {
        T Get(string name);
    }
}
