using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Plugins;

public class RuntimePluginPrioritizer : IRuntimePluginPrioritizer
{
    public IReadOnlyList<string> Prioritize(IReadOnlyList<string> pluginList)
    {
        return pluginList.OrderBy(x => x).ToList();
    }
}