using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDateTimeOffsetValueRetriever : IValueRetriever
    {
        private readonly Func<string, DateTimeOffset> dateTimeOffsetValueRetriever;

        public NullableDateTimeOffsetValueRetriever(Func<string, DateTimeOffset> dateTimeOffsetValueRetriever)
        {
            this.dateTimeOffsetValueRetriever = dateTimeOffsetValueRetriever;
        }

        public DateTimeOffset? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return dateTimeOffsetValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type type)
        {
            return type == typeof(DateTimeOffset);
        }
    }
}