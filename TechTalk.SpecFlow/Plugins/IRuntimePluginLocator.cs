using System.Collections.Generic;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePluginLocator
    {
        IReadOnlyList<string> GetAllRuntimePlugins();
    }
}
