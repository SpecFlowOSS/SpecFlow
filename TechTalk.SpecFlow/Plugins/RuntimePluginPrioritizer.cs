using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Plugins;

public class RuntimePluginPrioritizer : IRuntimePluginPrioritizer
{
    public IReadOnlyList<string> Prioritize(IReadOnlyList<string> pluginList)
    {
        if (pluginList is List<string> list)
        {
            list.Sort();
            return list;
        }

        return pluginList.OrderBy(x => x).ToList();
    }
}