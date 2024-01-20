using System.Collections.Generic;

namespace TechTalk.SpecFlow.Plugins;

public interface IRuntimePluginPrioritizer
{
    IReadOnlyList<string> Prioritize(IReadOnlyList<string> pluginPaths);
}