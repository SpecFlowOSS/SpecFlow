using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeOffsetValueRetriever : StructRetriever<DateTimeOffset>
    {
        protected override DateTimeOffset GetNonEmptyValue(string value)
        {
            DateTimeOffset.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTimeOffset returnValue);
            return returnValue;
        }
    }
}