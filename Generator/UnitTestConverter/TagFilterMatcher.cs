using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    internal class TagFilterMatcher : ITagFilterMatcher
    {
        private const string TAG_PREFIX_SEPARATOR = ":";
        private const StringComparison tagComparison = StringComparison.InvariantCultureIgnoreCase;

        private string GetExpectedTagName(string tagFilter)
        {
            return tagFilter.StartsWith("@") ? tagFilter.Substring(1) : tagFilter;
        }

        public bool Match(string tagFilter, IEnumerable<string> tagNames)
        {
            string expectedTagName = GetExpectedTagName(tagFilter);

            return tagNames != null &&
                   tagNames.Any(t => MatchExactTag(t, expectedTagName));
        }

        public bool MatchPrefix(string tagFilter, IEnumerable<string> tagNames)
        {
            string expectedTagName = GetExpectedTagName(tagFilter);
            string expectedTagPrefix = GetExpectedTagPrefix(expectedTagName); // we precalculate it to speed up comparison

            return tagNames != null &&
                   tagNames.Any(t => MatchTag(t, expectedTagName, expectedTagPrefix));
        }

        private static string GetExpectedTagPrefix(string expectedTagName)
        {
            return expectedTagName + TAG_PREFIX_SEPARATOR;
        }

        private bool MatchTag(string tagName, string expectedTagName, string expectedTagPrefix)
        {
            return
                MatchExactTag(tagName, expectedTagName) ||
                MatchTagPrefix(tagName, expectedTagPrefix);
        }

        private bool MatchExactTag(string tagName, string expectedTagName)
        {
            return tagName != null &&
                   tagName.Equals(expectedTagName, tagComparison);
        }

        private bool MatchTagPrefix(string tagName, string expectedTagPrefix)
        {
            return tagName != null &&
                   tagName.StartsWith(expectedTagPrefix, tagComparison);
        }

        private IEnumerable<string> GetTagValuesInternal(string tagFilter, IEnumerable<string> tagNames)
        {
            string expectedTagName = GetExpectedTagName(tagFilter);
            string expectedTagPrefix = GetExpectedTagPrefix(expectedTagName); // we precalculate it to speed up comparison

            if (tagNames == null)
                return Enumerable.Empty<string>();

            var tagsWithValue = tagNames.Where(t => MatchTag(t, expectedTagName, expectedTagPrefix));

            return tagsWithValue.Select(tagWithValue => MatchTagPrefix(tagWithValue, expectedTagPrefix) ? GetValue(expectedTagPrefix, tagWithValue) : "");
        }

        private string GetValue(string expectedTagPrefix, string tagWithValue)
        {
            return tagWithValue.Substring(expectedTagPrefix.Length);
        }

        public bool GetTagValue(string tagFilter, IEnumerable<string> tagNames, out string value)
        {
            value = GetTagValuesInternal(tagFilter, tagNames).FirstOrDefault();
            return value != null;
        }

        public string[] GetTagValues(string tagFilter, IEnumerable<string> tagNames)
        {
            return GetTagValuesInternal(tagFilter, tagNames).ToArray();
        }
    }
}