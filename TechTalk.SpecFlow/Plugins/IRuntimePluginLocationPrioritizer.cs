using System.Collections.Generic;

namespace TechTalk.SpecFlow.Plugins;

public interface IRuntimePluginLocationPrioritizer
{
    IReadOnlyList<string> Prioritize(IReadOnlyList<string> pluginPaths);
}