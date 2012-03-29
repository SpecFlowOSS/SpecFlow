using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

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

        public static bool MatchPrefix(this ITagFilterMatcher tagFilterMatcher, string tagFilter, Feature feature)
        {
            if (feature.Tags == null)
                return false;

            return tagFilterMatcher.MatchPrefix(tagFilter, feature.Tags.Select(t => t.Name));
        }

        public static bool GetTagValue(this ITagFilterMatcher tagFilterMatcher, string tagFilter, Feature feature, out string value)
        {
            if (feature.Tags == null)
            {
                value = null;
                return false;
            }

            return tagFilterMatcher.GetTagValue(tagFilter, feature.Tags.Select(t => t.Name), out value);
        }
    }
}