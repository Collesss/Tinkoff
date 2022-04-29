using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text;

namespace ExtensionPlugins
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }
    }
}
