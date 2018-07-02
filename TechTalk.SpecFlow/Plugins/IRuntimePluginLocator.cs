using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Plugins
{
    interface IRuntimePluginLocator
    {
        IReadOnlyList<string> GetAllRuntimePlugins();
    }
}
