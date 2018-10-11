using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeValueRetriever : NonNullableValueRetriever<DateTime>
    {
        public override DateTime GetValue(string value)
        {
            DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime returnValue);
            return returnValue;
        }
    }
}