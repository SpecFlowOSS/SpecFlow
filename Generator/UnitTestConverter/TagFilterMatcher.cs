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

        private string GetExpectedTagName(string tagFilter)
        {
            return tagFilter.StartsWith("@") ? tagFilter.Substring(1) : tagFilter;
        }

        public bool Match(string tagFilter, Feature feature)
        {
            string expectedTagName = GetExpectedTagName(tagFilter);
            string expectedTagPrefix = GetExpectedTagPrefix(expectedTagName); // we precalculate it to speed up comparison

            return feature.Tags != null &&
                   feature.Tags.Any(t => MatchTag(t, expectedTagName, expectedTagPrefix));
        }

        private static string GetExpectedTagPrefix(string expectedTagName)
        {
            return expectedTagName + TAG_PREFIX_SEPARATOR;
        }

        private bool MatchTag(Tag tag, string expectedTagName, string expectedTagPrefix)
        {
            return
                (tag.Name != null && tag.Name.Equals(expectedTagName, StringComparison.InvariantCultureIgnoreCase)) ||
                MatchTagPrefix(tag, expectedTagPrefix);
        }

        private bool MatchTagPrefix(Tag tag, string expectedTagPrefix)
        {
            return
                tag.Name != null &&
                tag.Name.StartsWith(expectedTagPrefix, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool GetTagValue(string tagFilter, Feature feature, out string value)
        {
            value = null;

            string expectedTagName = GetExpectedTagName(tagFilter);
            string expectedTagPrefix = GetExpectedTagPrefix(expectedTagName); // we precalculate it to speed up comparison

            if (feature.Tags == null)
                return false;

            var tagWithValue = feature.Tags.FirstOrDefault(t => MatchTag(t, expectedTagName, expectedTagPrefix));
            if (tagWithValue == null)
                return false;

            value = MatchTagPrefix(tagWithValue, expectedTagPrefix) ? tagWithValue.Name.Substring(expectedTagPrefix.Length) : "";
            return true;
        }
    }
}