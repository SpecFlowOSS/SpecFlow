using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeOffsetValueRetriever : NonNullableValueRetriever<DateTimeOffset>
    {
        public override DateTimeOffset GetValue(string value)
        {
            DateTimeOffset.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTimeOffset returnValue);
            return returnValue;
        }
    }
}