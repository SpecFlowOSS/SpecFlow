using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDateTimeOffsetValueRetriever : IValueRetriever
    {
        private readonly Func<string, DateTimeOffset> dateTimeOffsetValueRetriever = v => new DateTimeOffsetValueRetriever().GetValue(v);

        public NullableDateTimeOffsetValueRetriever(Func<string, DateTimeOffset> dateTimeOffsetValueRetriever = null)
        {
            if (dateTimeOffsetValueRetriever != null)
                this.dateTimeOffsetValueRetriever = dateTimeOffsetValueRetriever;
        }

        public virtual DateTimeOffset? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return dateTimeOffsetValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(DateTimeOffset?);
        }
    }
}