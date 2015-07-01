using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDateTimeValueRetriever : IValueRetriever
    {
        private readonly Func<string, DateTime> dateTimeValueRetriever = v => new DateTimeValueRetriever().GetValue(v);

        public NullableDateTimeValueRetriever(Func<string, DateTime> dateTimeValueRetriever = null)
        {
            if (dateTimeValueRetriever != null)
                this.dateTimeValueRetriever = dateTimeValueRetriever;
        }

        public DateTime? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return dateTimeValueRetriever(value);
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public bool CanRetrieve(TableRow row, Type type)
        {
            return type == typeof(DateTime?);
        }
    }
}