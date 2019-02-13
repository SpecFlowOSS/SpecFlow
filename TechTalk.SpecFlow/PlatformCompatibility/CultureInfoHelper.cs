using System.Globalization;

namespace TechTalk.SpecFlow.Compatibility
{
	internal static class CultureInfoHelper
    {
        public static CultureInfo GetCultureInfo(string cultureName)
        {
            return CultureInfo.GetCultureInfo(cultureName);
        }
    }
}
