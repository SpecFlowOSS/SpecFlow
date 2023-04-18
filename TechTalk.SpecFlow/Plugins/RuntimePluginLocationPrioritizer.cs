using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Plugins;

public class RuntimePluginLocationPrioritizer : IRuntimePluginLocationPrioritizer
{
    public IReadOnlyList<string> Prioritize(IReadOnlyList<string> pluginList)
    {
        return pluginList.OrderBy(x => x).ToList();
    }
}