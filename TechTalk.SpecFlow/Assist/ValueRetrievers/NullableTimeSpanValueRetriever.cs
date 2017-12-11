using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableTimeSpanValueRetriever : IValueRetriever
    {
        private readonly Func<string, TimeSpan?> dateTimeValueRetriever = v => new TimeSpanValueRetriever().GetValue(v);

        public NullableTimeSpanValueRetriever(Func<string, TimeSpan?> dateTimeValueRetriever = null)
        {
            if (dateTimeValueRetriever != null)
                this.dateTimeValueRetriever = dateTimeValueRetriever;
        }

        public virtual TimeSpan? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return dateTimeValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(TimeSpan?);
        }
    }
}