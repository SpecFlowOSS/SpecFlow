using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.Plugins
{
    public class RuntimePluginLocationMerger : IRuntimePluginLocationMerger
    {
        public IReadOnlyList<string> Merge(IReadOnlyList<string> pluginPaths)
        {
            var sortedPluginPaths = pluginPaths.OrderBy(Path.GetFileName).ToList();
            // Idea is to filter out the same assemblies stored on different paths. Shortcut: check if we even have duplicated assemblies
            var hashset = new HashSet<string>(
#if NETCOREAPP2_1_OR_GREATER
                // initialize with expected size when available
                sortedPluginPaths.Count
#endif
                );

            List<string> modifiedList = null;
            for (var i = 0; i < sortedPluginPaths.Count; i++)
            {
                if (hashset.Add(Path.GetFileName(sortedPluginPaths[i])))
                {
                    // Assembly not duplicated
                    if (modifiedList is null)
                    {
                        // No duplication yet, continue
                        continue;
                    }

                    // We already had a duplication, fill the list with this non duplicated entry
                    modifiedList.Add(sortedPluginPaths[i]);
                }
                else if (modifiedList is null)
                {
                    // First duplicated assembly, Copy all previous entry into the new list
                    modifiedList = CopyUntilIndex(sortedPluginPaths, i);
                }
            }

            return modifiedList ?? sortedPluginPaths;
        }

        private static List<string> CopyUntilIndex(IReadOnlyList<string> pluginPaths, int index)
        {
            var result = new List<string>(pluginPaths.Count);
            for (var i = 0; i < index; i++)
            {
                result.Add(pluginPaths[i]);
            }
            return result;
        }
    }
}