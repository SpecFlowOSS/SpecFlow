using System;

namespace TechTalk.SpecFlow
{
	internal static class StringExtensions
    {
        internal static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        internal static bool IsNotNullOrEmpty(this string value)
        {
            return !String.IsNullOrEmpty(value);
        }

        internal static bool IsNullOrWhiteSpace(this String value)
        {
            if (value == null) return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!Char.IsWhiteSpace(value[i])) return false;
            }

            return true;
        }

        internal static bool IsNotNullOrWhiteSpace(this String value)
        {
            return !value.IsNullOrWhiteSpace();
        }

        internal static string StripWhitespaces(this String value)
        {
            return value.Replace(" ", "").Replace("\n", "").Replace("\r", "");
        }
    }
}
