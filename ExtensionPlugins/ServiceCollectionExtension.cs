using Filter.Union;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NamedRegistrarDependency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExtensionPlugins
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddUsePlugin<T>(this IServiceCollection serviceCollection, OptionsPlugins configuration)
        {
            foreach (var loadAssemblyInfo in configuration.Assemblies)
            {
                string fullPathToAssembly = Path.GetFullPath(Path.Combine(configuration.PathLoad, loadAssemblyInfo.PathToAssembly));
                PluginLoadContext pluginLoadContext = new PluginLoadContext(fullPathToAssembly);

                Assembly loadingAssembly = pluginLoadContext.LoadFromAssemblyPath(fullPathToAssembly);

                foreach (var optionInfo in loadAssemblyInfo.Options)
                {
                    var optionType = loadingAssembly.GetType(optionInfo.Type, true);

                    serviceCollection.Configure(optionType, optionInfo.Data);
                }

                foreach (var implementation in loadAssemblyInfo.Classes)
                {
                    Type implementationType = loadingAssembly.GetType(implementation, true);
                    //filterUnionOptions.UsingFilter.Add(realization.AssemblyQualifiedName);
                    serviceCollection.AddNamedDependency(typeof(T), implementationType, implementationType.AssemblyQualifiedName, ServiceLifetime.Transient);
                    //serviceCollection.AddTransient(baseType, realization);
                }
            }

            //serviceCollection.AddSingleton(Options.Create(filterUnionOptions));
            //serviceCollection.ConfigureOptions(Options.Create(filterUnionOptions));
            //serviceCollection.Configure<FilterUnionOptions>(fUO => fUO.UsingFilter = filterUnionOptions.UsingFilter);

            return serviceCollection;
        }

        public static IServiceCollection AddUsePlugin(this IServiceCollection serviceCollection, OptionsPlugins configuration, Type baseType) =>
            typeof(ServiceCollectionExtension)
                .GetMethod("AddUsePlugin", 1, new Type[] { typeof(IServiceCollection), typeof(OptionsPlugins) })
                .MakeGenericMethod(baseType)
                .Invoke(null, new object[] { serviceCollection, configuration }) as IServiceCollection;

        private static IServiceCollection Configure(this IServiceCollection serviceCollection, Type configureType, IConfiguration configuration) =>
            typeof(OptionsConfigurationServiceCollectionExtensions)
                        .GetMethod("Configure", 1, new Type[] { typeof(IServiceCollection), typeof(IConfiguration) })
                        .MakeGenericMethod(configureType)
                        .Invoke(null, new object[] { serviceCollection, configuration }) as IServiceCollection;
    }
}
