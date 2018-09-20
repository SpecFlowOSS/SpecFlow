using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class TimeSpanValueRetriever : NonNullableValueRetriever<TimeSpan>
    {
        public override TimeSpan GetValue(string value)
        {
            return TimeSpan.Parse(value, CultureInfo.CurrentCulture);
        }
    }
}