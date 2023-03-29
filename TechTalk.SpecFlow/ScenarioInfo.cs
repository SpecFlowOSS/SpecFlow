using System;
using System.Collections.Specialized;
using System.Linq;

namespace TechTalk.SpecFlow
{
    /// <summary>
    /// Contains information about the scenario currently being executed.
    /// </summary>
    public class ScenarioInfo
    {
        /// <summary>
        /// Direct tags of the scenario, including tags of the examples block.
        /// </summary>
        public string[] Tags { get; }
        /// <summary>
        /// Contains direct tags and tags inherited from the feature and the rule.
        /// </summary>
        public string[] CombinedTags { get; private set; }

        [Obsolete($"Deprecated. Use '{nameof(CombinedTags)}' instead.", false)]
        public string[] ScenarioAndFeatureTags => CombinedTags;
        
        /// <summary>
        /// The arguments used to execute a scenario outline example.
        /// </summary>
        public IOrderedDictionary Arguments { get; }

        /// <summary>
        /// The title (name) of the scenario.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The description of the scenario.
        /// </summary>
        public string Description { get; }

        public ScenarioInfo(string title, string description, string[] tags, IOrderedDictionary arguments, string[] inheritedTags = null)
        {
            Title = title;
            Description = description;
            Tags = tags ?? Array.Empty<string>();
            Arguments = arguments;
            CombinedTags = Tags.Concat(inheritedTags ?? Array.Empty<string>()).ToArray();
        }
    }
}