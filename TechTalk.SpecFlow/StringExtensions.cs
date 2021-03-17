namespace TechTalk.SpecFlow
{
    internal static class StringExtensions
    {
        internal static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        internal static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        internal static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        internal static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        internal static string StripWhitespaces(this string value)
        {
            return value.Replace(" ", "").Replace("\n", "").Replace("\r", "");
        }
    }
}
