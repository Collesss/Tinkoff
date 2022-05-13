using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamedRegistrarDependency
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNamedDependency<T, V>(this IServiceCollection sc, string name, ServiceLifetime lifetime) where V : T
        {
            sc.Add(new ServiceDescriptor(typeof(INamedDependency<T>), sp => new NamedDependency<T>(name, sp.GetRequiredService<V>()), ServiceLifetime.Transient));

            sc.Add(new ServiceDescriptor(typeof(V), typeof(V), lifetime));

            ServiceDescriptor unionNamedDepency = new ServiceDescriptor(typeof(IUnionNamedDependency<T>), typeof(UnionNamedDependency<T>), ServiceLifetime.Transient);

            if (!sc.Contains(unionNamedDepency))
                sc.Add(unionNamedDepency);

            return sc;
        }

        public static IServiceCollection AddNamedDependency(this IServiceCollection sc, Type baseType, Type realization, string name, ServiceLifetime lifetime)
        {
            Type typeGenericINamedDepency = typeof(INamedDependency<>).MakeGenericType(baseType);
            Type typeGenericNamedDepency = typeof(NamedDependency<>).MakeGenericType(baseType);

            sc.Add(new ServiceDescriptor(realization, realization, lifetime));

            sc.Add(new ServiceDescriptor(typeGenericINamedDepency, sp =>
                typeGenericNamedDepency.GetConstructor(new Type[] { typeof(string), baseType })
                .Invoke(new object[] { name, sp.GetRequiredService(realization) }), ServiceLifetime.Transient));

            Type iunionType = typeof(IUnionNamedDependency<>).MakeGenericType(baseType);
            Type unionType = typeof(UnionNamedDependency<>).MakeGenericType(baseType);

            ServiceDescriptor unionNamedDepency = new ServiceDescriptor(iunionType, unionType, ServiceLifetime.Transient);

            if (!sc.Contains(unionNamedDepency))
                sc.Add(unionNamedDepency);

            return sc;
        }
    }
}
