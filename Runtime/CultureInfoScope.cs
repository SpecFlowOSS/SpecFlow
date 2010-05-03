using System;
using System.Globalization;
using System.Threading;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow
{
    public class CultureInfoScope : IDisposable
    {
        private readonly CultureInfo originalCultureInfo;

        public CultureInfoScope(CultureInfo cultureInfo)
        {
            if (cultureInfo != null && !cultureInfo.Equals(Thread.CurrentThread.CurrentCulture))
            {
                if (cultureInfo.IsNeutralCulture)
                {
                    cultureInfo = LanguageHelper.GetSpecificCultureInfo(cultureInfo);
                }
                originalCultureInfo = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = cultureInfo;
            }
        }

        public void Dispose()
        {
            if (originalCultureInfo != null)
                Thread.CurrentThread.CurrentCulture = originalCultureInfo;
        }
    }
}