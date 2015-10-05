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

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type type)
        {
            return type == typeof(DateTime?);
        }
    }
}