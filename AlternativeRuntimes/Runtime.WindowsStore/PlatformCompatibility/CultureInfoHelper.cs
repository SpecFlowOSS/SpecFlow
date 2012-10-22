
namespace TechTalk.SpecFlow.Compatibility
{
    using System.Globalization;

    internal static class CultureInfoHelper
    {
        public static CultureInfo GetCultureInfo(string cultureName)
        {
            return new CultureInfo(cultureName);
        }
    }
}
