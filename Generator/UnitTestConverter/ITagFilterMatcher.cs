using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface ITagFilterMatcher
    {
        bool Match(string tagFilter, IEnumerable<string> tagNames);
        bool MatchPrefix(string tagFilter, IEnumerable<string> tagNames);
        bool GetTagValue(string tagFilter, IEnumerable<string> tagNames, out string value);
        string[] GetTagValues(string tagFilter, IEnumerable<string> tagNames);
    }

    public static class TagFilterMatcherExtensions
    {
        public static bool Match(this ITagFilterMatcher tagFilterMatcher, string tagFilter, string tagName)
        {
            if (tagName == null)
                return false;

            return tagFilterMatcher.Match(tagFilter, new string[] {tagName});
        }

        public static bool MatchPrefix(this ITagFilterMatcher tagFilterMatcher, string tagFilter, SpecFlowFeature feature)
        {
            return tagFilterMatcher.MatchPrefix(tagFilter, feature.Tags.Select(t => t.GetNameWithoutAt()));
        }

        public static bool GetTagValue(this ITagFilterMatcher tagFilterMatcher, string tagFilter, SpecFlowFeature feature, out string value)
        {
            return tagFilterMatcher.GetTagValue(tagFilter, feature.Tags.Select(t => t.GetNameWithoutAt()), out value);
        }
    }
}