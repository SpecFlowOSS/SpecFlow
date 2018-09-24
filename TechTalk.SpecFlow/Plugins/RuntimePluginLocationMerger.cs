using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.Plugins
{
    public class RuntimePluginLocationMerger : IRuntimePluginLocationMerger
    {
        public IReadOnlyList<string> Merge(IReadOnlyList<string> pluginPaths)
        {
            var merge = from entry in pluginPaths
                        group entry by Path.GetFileName(entry)
                into g
                        select g.FirstOrDefault();
            return merge.ToList();
        }
    }
}