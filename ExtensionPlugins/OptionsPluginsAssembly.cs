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
            public string PathToAssembly { get; set; }
            public string[] Classes { get; set; }
            public OptionsPluginsAssemblyOption[] Options { get; set; }
        }
    }
}
