using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeValueRetriever : StructRetriever<DateTime>
    {
        /// <summary>
        /// Gets or sets the DateTimeStyles to use when parsing the string value.
        /// </summary>
        /// <remarks>Defaults to DateTimeStyles.None.</remarks>
        public static DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.None;

        protected override DateTime GetNonEmptyValue(string value)
        {
            DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles, out DateTime returnValue);
            return returnValue;
        }
    }
}