using System;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface ITagFilterMatcher
    {
        bool Match(string tagFilter, Feature feature);
    }

    internal class TagFilterMatcher : ITagFilterMatcher
    {
        private const string TAG_PREFIX_SEPARATOR = ":";

        public bool Match(string tagFilter, Feature feature)
        {
            string expectedTagName = tagFilter.StartsWith("@") ? tagFilter.Substring(1) : tagFilter;
            string expectedTagPrefix = expectedTagName + TAG_PREFIX_SEPARATOR; // we precalculate it to speed up comparison

            return feature.Tags != null &&
                   feature.Tags.Any(t => MatchTag(t, expectedTagName, expectedTagPrefix));
        }

        private static bool MatchTag(Tag tag, string expectedTagName, string expectedTagPrefix)
        {
            return
                tag.Name != null &&
                (
                    tag.Name.Equals(expectedTagName, StringComparison.InvariantCultureIgnoreCase) ||
                    tag.Name.StartsWith(expectedTagPrefix, StringComparison.InvariantCultureIgnoreCase)
                );
        }
    }
}