using System.Linq;

namespace TechTalk.SpecFlow
{
    #nullable enable
    /// <summary>
    /// Provides helper methods around tags.
    /// </summary>
    public static class TagHelper
    {
        /// <summary>
        /// Checks whether the supplied tags contain the ignore tag
        /// </summary>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A boolean that indicates whether or not the ignore tag is present.</returns>
        public static bool ContainsIgnoreTag(string[]? tags)
        {
            if (tags is null)
            {
                return false;
            }

            for (int i = 0; i < tags.Length; i++)
            {
                if (string.Equals(tags[i], "ignore", System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Combines feature tags and rule tags in a way that both can be null or empty. Duplicates are removed.
        /// </summary>
        public static string[]? CombineTags(string[]? featureTags, string[]? ruleTags)
        {
            if (featureTags == null) return ruleTags;
            if (ruleTags == null) return featureTags;
            return featureTags.Concat(ruleTags).Distinct().ToArray();
        }
    }
}
