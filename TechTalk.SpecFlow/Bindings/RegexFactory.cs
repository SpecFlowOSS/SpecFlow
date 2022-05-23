using System;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    internal static class RegexFactory
    {
        private static RegexOptions RegexOptions = RegexOptions.CultureInvariant;

        public static Regex CreateWholeTextRegexForBindings(string regexString) => CreateRegexForBindings(GetWholeTextMatchRegexSource(regexString));

        public static string GetWholeTextMatchRegexSource(string regexString)
        {
            if (regexString == null)
                throw new ArgumentNullException(nameof(regexString));

            if (!regexString.StartsWith("^")) regexString = "^" + regexString;
            if (!regexString.EndsWith("$")) regexString += "$";
            return regexString;
        }

        public static Regex CreateRegexForBindings(string regexString)
        {
            if (regexString == null)
                throw new ArgumentNullException(nameof(regexString));

            return new Regex(regexString, RegexOptions);
        }
    }
}
