using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePluginLocator
    {
        IReadOnlyList<string> GetAllRuntimePlugins();
    }
}
