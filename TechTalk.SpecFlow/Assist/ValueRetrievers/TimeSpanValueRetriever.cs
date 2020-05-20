using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class TimeSpanValueRetriever : StructRetriever<TimeSpan>
    {
        protected override TimeSpan GetNonEmptyValue(string value)
        {
            TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out var result);
            return result;
        }
    }
}