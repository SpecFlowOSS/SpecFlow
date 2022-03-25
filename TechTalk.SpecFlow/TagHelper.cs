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
    }
}
