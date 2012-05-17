using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    internal static class RegexFactory
    {
#if SILVERLIGHT
        private static RegexOptions RegexOptions = RegexOptions.CultureInvariant;
#else
        private static RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
#endif

        public static Regex Create(string regexString)
        {
            return new Regex("^" + regexString + "$", RegexOptions);
        }
    }
}