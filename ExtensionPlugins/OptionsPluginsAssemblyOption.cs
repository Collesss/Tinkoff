using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionPlugins
{
    public partial class OptionsPlugins
    {
        public partial class OptionsPluginsAssembly
        {
            public class OptionsPluginsAssemblyOption
            {
                public string Type { get; set; }
                public IConfigurationSection Data { get; set; }
            }
        }
    }
}
