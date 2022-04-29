using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionPlugins
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection AddUsePlugin(this ServiceCollection serviceCollection)
        {



            return serviceCollection;
        }
    }
}
