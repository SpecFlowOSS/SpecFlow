using System;
using System.Collections.Specialized;
using System.Linq;

namespace TechTalk.SpecFlow
{
    public class ScenarioInfo
    {
        /// <summary>
        /// Direct tags of the scenario.
        /// </summary>
        public string[] Tags { get; private set; }
        /// <summary>
        /// Contains direct tags and tags inherited from the feature and the rule.
        /// </summary>
        public string[] CombinedTags { get; private set; }

        [Obsolete($"Deprecated. Use '{nameof(CombinedTags)}' instead.")]
        public string[] ScenarioAndFeatureTags => CombinedTags;
        public IOrderedDictionary Arguments { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }

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