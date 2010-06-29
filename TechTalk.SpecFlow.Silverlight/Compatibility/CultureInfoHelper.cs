using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Compatibility
{
    internal static class CultureInfoHelper
    {
        public static CultureInfo GetCultureInfo(string cultureName)
        {
            return new CultureInfo(cultureName);
        }
    }
}
