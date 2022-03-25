using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeOffsetValueRetriever : StructRetriever<DateTimeOffset>
    {
        /// <summary>
        /// Gets or sets the DateTimeStyles to use when parsing the string value.
        /// </summary>
        /// <remarks>Defaults to DateTimeStyles.None.</remarks>
        public static DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.None;

        protected override DateTimeOffset GetNonEmptyValue(string value)
        {
            DateTimeOffset.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles, out DateTimeOffset returnValue);
            return returnValue;
        }
    }
}