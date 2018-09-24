using System.Collections.Generic;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePluginLocationMerger
    {
        IReadOnlyList<string> Merge(IReadOnlyList<string> pluginPaths);
    }
}