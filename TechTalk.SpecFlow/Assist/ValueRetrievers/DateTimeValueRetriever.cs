using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeValueRetriever : StructRetriever<DateTime>
    {
        protected override DateTime GetNonEmptyValue(string value)
        {
            DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime returnValue);
            return returnValue;
        }
    }
}