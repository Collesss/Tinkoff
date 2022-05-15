﻿using Filter.Union;
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
        public static IServiceCollection AddUsePlugin(this IServiceCollection serviceCollection, IConfiguration configuration, Type baseType)
        {
            string pathLoad = configuration["PathLoad"];

            foreach (var loadAssemblyInfo in configuration.GetSection("Assemblies").GetChildren())
            {
                string fullPathToAssembly = Path.GetFullPath(Path.Combine(pathLoad, loadAssemblyInfo["PathToAssembly"]));
                PluginLoadContext pluginLoadContext = new PluginLoadContext(fullPathToAssembly);

                Assembly loadingAssembly = pluginLoadContext.LoadFromAssemblyPath(fullPathToAssembly);

                var realizations = loadAssemblyInfo.GetSection("UsingClassesFilters").GetChildren()
                    .Select(typeName => loadingAssembly.GetType(typeName.Value, true));

                foreach (var optionInfo in loadAssemblyInfo.GetSection("UsingClassesOptions").GetChildren())
                {
                    var optionType = loadingAssembly.GetType(optionInfo["OptionClass"], true);
                    var optionData = optionInfo.GetSection("Data");

                    //serviceCollection

                    typeof(OptionsConfigurationServiceCollectionExtensions)
                        .GetMethod("Configure", 1, new Type[] { typeof(IServiceCollection), typeof(IConfiguration) })
                        .MakeGenericMethod(optionType)
                        .Invoke(null, new object[] { serviceCollection, optionData });
                    
                }

                foreach (Type realization in realizations)
                {
                    //filterUnionOptions.UsingFilter.Add(realization.AssemblyQualifiedName);
                    serviceCollection.AddNamedDependency(baseType, realization, realization.AssemblyQualifiedName, ServiceLifetime.Transient);
                    //serviceCollection.AddTransient(baseType, realization);
                }
            }

            //serviceCollection.AddSingleton(Options.Create(filterUnionOptions));
            //serviceCollection.ConfigureOptions(Options.Create(filterUnionOptions));
            //serviceCollection.Configure<FilterUnionOptions>(fUO => fUO.UsingFilter = filterUnionOptions.UsingFilter);

            return serviceCollection;
        }
    }
}
