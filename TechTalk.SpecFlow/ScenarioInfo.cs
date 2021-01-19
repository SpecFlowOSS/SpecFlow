using System.Collections.Specialized;
using System.Linq;

namespace TechTalk.SpecFlow
{
    public class ScenarioInfo
    {
        public string[] Tags { get; private set; }
        public string[] ScenarioAndFeatureTags { get; private set; }
        public IOrderedDictionary Arguments { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public ScenarioInfo(string title, string description, string[] tags, IOrderedDictionary arguments, params string[] featureTags)
        {
            Title = title;
            Description = description;
            Tags = tags ?? new string[0];
            Arguments = arguments;
            ScenarioAndFeatureTags = Tags.Concat(featureTags ?? new string[0]).ToArray();
        }
    }
}